using Microsoft.AspNet.Identity;
using RipCore.Models;
using RipCore.Models.Entities;
using RipCore.Models.ViewModels;
using RipCore.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace RipCore.Controllers
{
    public class SolutionController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private SolutionService sService = new SolutionService();
        private AssignmentsService aAssignment = new AssignmentsService();
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SubmitSolution(AssignmentViewModel viewModel)
        {
            SubmissionViewModel submission = new SubmissionViewModel { AssignmentName = viewModel.Title, MilestoneID = viewModel.milestoneSubmissionID  };
            Assignment assigment = (from s in db.Assignments
                                    where s.ID == viewModel.ID
                                    select s).FirstOrDefault();
            submission.ProgrammingLanguage = aAssignment.GetProgrammingLanguageByID(assigment.ProgrammingLanguageID);
            if (viewModel.File != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    viewModel.File.InputStream.CopyTo(memoryStream);

                    string result = Encoding.ASCII.GetString(memoryStream.ToArray());
                    submission.Code = result;
                }
            }
            else if (!string.IsNullOrEmpty(viewModel.Solution))
            {
                submission.Code = viewModel.Solution;
            }

            if (submission.ProgrammingLanguage == "regex")
            {
                return RedirectToAction("RegexTest", submission);
            }

            if (submission.ProgrammingLanguage == "other")
            {
                return RedirectToAction("Other", submission);
            }

            if (submission.ProgrammingLanguage == "otherNotTests")
            {
                return RedirectToAction("OtherWithTests", submission);
            }
            return RedirectToAction("CompileSolution", submission);
        }

        ///[HttpPost] // ??
        [ValidateInput(false)]
        public ActionResult CompileSolution(SubmissionViewModel data)
        {
            var code = data.Code;
            data.ExpectedOutput = new List<string>();
            data.SolutionOutput = new List<string>();
            string user = User.Identity.GetUserId();

            string smu = AppDomain.CurrentDomain.BaseDirectory;

            var workingFolder = smu + user + "\\"; //name; // eða ID
            System.IO.Directory.CreateDirectory(workingFolder);

            var cppFileName = data.AssignmentName.Replace(" ", "").ToLower() + data.ProgrammingLanguage; // ---- Verkefnaheiti
            var exeFilePath = workingFolder + data.AssignmentName.Replace(" ", "").ToLower() + ".exe"; // ----- verkefnaheiti

            // Write the code to a file, such that the compiler
            // can find it:
            System.IO.File.WriteAllText(workingFolder + cppFileName, code);

            // In this case, we use the C++ compiler (cl.exe) which ships
            // with Visual Studio. It is located in this folder:
            string compilerFolder;
            if (data.ProgrammingLanguage == ".cs")
            {
                compilerFolder = "C:\\Windows\\Microsoft.NET\\Framework\\v3.5\\";
            }
            else
            {
                compilerFolder = "C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\VC\\bin\\";
            }
            // There is a bit more to executing the compiler than
            // just calling cl.exe. In order for it to be able to know
            // where to find #include-d files (such as <iostream>),
            // we need to add certain folders to the PATH.
            // There is a .bat file which does that, and it is
            // located in the same folder as cl.exe, so we need to execute
            // that .bat file first.

            // Using this approach means that:
            // * the computer running our web application must have
            //   Visual Studio installed. This is an assumption we can
            //   make in this project.
            // * Hardcoding the path to the compiler is not an optimal
            //   solution. A better approach is to store the path in
            //   web.config, and access that value using ConfigurationManager.AppSettings.

            // Execute the compiler:
            Process compiler = new Process();
            if (data.ProgrammingLanguage == ".cs")
            {
                compiler.StartInfo.FileName = "cmd.exe";
                compiler.StartInfo.WorkingDirectory = workingFolder;
                compiler.StartInfo.RedirectStandardInput = true;
                compiler.StartInfo.RedirectStandardError = true;
                compiler.StartInfo.RedirectStandardOutput = true;
                compiler.StartInfo.UseShellExecute = false;

                compiler.Start();
                compiler.StandardInput.WriteLine("C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\CSC.EXE " + cppFileName);
                //compiler.StandardInput.WriteLine("cl.exe /nologo /EHsc " + cppFileName);
                compiler.StandardInput.WriteLine("exit");
                //string output = compiler.StandardOutput.ReadToEnd();
                Debug.WriteLine(compiler.StandardError.ReadLine());
            }
            
            else
            {                
                compiler.StartInfo.FileName = "cmd.exe";
                compiler.StartInfo.WorkingDirectory = workingFolder;
                compiler.StartInfo.RedirectStandardInput = true;
                compiler.StartInfo.RedirectStandardOutput = true;
                compiler.StartInfo.UseShellExecute = false;

                compiler.Start();
                compiler.StandardInput.WriteLine("\"" + compilerFolder + "vcvars32.bat" + "\"");
                compiler.StandardInput.WriteLine("cl.exe /nologo /EHsc " + cppFileName);
                compiler.StandardInput.WriteLine("exit");
                string output = compiler.StandardOutput.ReadToEnd();
            }
          //  var compilerTest = compiler.WaitForExit(10000);
            //if (!compilerTest)
            //{
                //data.SolutionOutput.Add("Compile Time Error!");
                //compiler.Kill();
                //return View(data);
            //}
            compiler.Close();


            // Check if the compile succeeded, and if it did,
            // we try to execute the code:
            if (System.IO.File.Exists(exeFilePath))
            {
                var processInfoExe = new ProcessStartInfo(exeFilePath, "");
                processInfoExe.UseShellExecute = false;
                processInfoExe.RedirectStandardOutput = true;
                processInfoExe.RedirectStandardInput = true;
                processInfoExe.RedirectStandardError = true;
                processInfoExe.CreateNoWindow = true;
                string entityOutput = "";
                string entityExpected = "";
                List <Tuple<string, string>> excpectedData = sService.GetExpectedData(data.MilestoneID);
                for (int i = 0; i < excpectedData.Count; i++)
                {
                    //Task.Factory.StartNew(() => { Thread.Sleep(10000); processExe.Kill(); });
                    using (var processExe = new Process())
                    {
                        processExe.StartInfo = processInfoExe;
                        processExe.Start();

                        if (excpectedData[i].Item1 != "")
                        {
                            processExe.StandardInput.WriteLine(excpectedData[i].Item1);
                        }
                        // In this example, we don't try to pass any input
                        // to the program, but that is of course also
                        // necessary. We would do that here, using
                        // processExe.StandardInput.WriteLine(), similar
                        // to above.
                        var RunTimeTest = processExe.WaitForExit(5000);
                        if (!RunTimeTest)
                        {
                            data.SolutionOutput.Add("Run Time Error!");
                            entityOutput += "Run Time Error";
                            processExe.Kill();
                            return View(data);
                        }

                        // We then read the output of the program:
                        string programOutput = "";
                        while (!processExe.StandardOutput.EndOfStream)
                        {
                            programOutput = processExe.StandardOutput.ReadLine();
                        }

                        ViewBag.Output = programOutput;
                        if (!String.IsNullOrEmpty(programOutput))
                        {
                            data.SolutionOutput.Add(programOutput);
                            entityOutput += programOutput + ' ';
                        }
                        data.ExpectedOutput.Add(excpectedData[i].Item2);
                        entityOutput += excpectedData[i].Item2 + ' ';
                        if (string.Equals(programOutput, excpectedData[i].Item2))
                        {
                            data.IsAccepted = true;
                        }
                        else
                        {
                            data.IsAccepted = false;
                            break;
                        }
                    }
                }

                Submission submission = new Submission { MilestoneID = data.MilestoneID, IsAccepted = data.IsAccepted, SolutionOutput = entityOutput, UserID = User.Identity.GetUserId(), Code = data.Code, ExpectedOutput = entityExpected};
                db.Submission.Add(submission);
                db.SaveChanges();

                Solution solution = (from s in db.Solutions where s.MilestoneID == submission.MilestoneID && s.StudentID == submission.UserID select s).FirstOrDefault();
                if(solution == null)
                {
                    Solution soliholm = new Solution { MilestoneID = submission.MilestoneID, StudentID = submission.UserID, SubmissionID = submission.ID, Code = data.Code }; 
                    db.Solutions.Add(soliholm);
                    db.SaveChanges();
                }
                else
                {
                    Submission bestSubmission = sService.GetSubmissionByID(solution.SubmissionID);
                    if (!bestSubmission.IsAccepted || (bestSubmission.IsAccepted && submission.IsAccepted))
                    {
                        solution.SubmissionID = submission.ID;
                        solution.Code = data.Code;
                        db.SaveChanges();
                    }
                    else
                    {
                        // do nothing
                    }
                } 
            }
            else
            {
                Submission submission = new Submission { MilestoneID = data.MilestoneID, IsAccepted = data.IsAccepted, SolutionOutput = "Please add an executable file!", UserID = User.Identity.GetUserId(), Code = data.Code, ExpectedOutput = "" };
                db.Submission.Add(submission);
                db.SaveChanges();
                data.SolutionOutput.Add("Please add an executable file!");
            }

            // TODO: We might want to clean up after the process, there
            // may be files we should delete etc.

            Directory.Delete(workingFolder, true); //Deletar moppunni sem vid gerdum adan

            return View(data);
        }

        public ActionResult AllSolutions(int id)
        {
            SolutionOverViewModel submissions = new SolutionOverViewModel { AssignmentSolutions = sService.GetSolutionsForView(id), MilestoneNames = sService.GetMilestoneNames(id) };
            return View(submissions);
        }

        public ActionResult AllSubmissions(int id)
        {
            string userID = User.Identity.GetUserId();
            SubmissionsOverViewModel submissions = new SubmissionsOverViewModel { otherSubmissions = sService.GetAllNotConnected(id, userID), usersSubmissions = sService.GetSubmissionsForUser(id, userID), MilestoneNames = sService.GetMilestoneNames(id) };
            return View(submissions);
        }

        public ActionResult SubmissionDetails(int id, bool isTeacher)
        {
            SubmissionViewModel submission = sService.GetSubmissionForView(id);
            string userID = User.Identity.GetUserId();
            Solution solution = (from s in db.Solutions where s.SubmissionID == submission.ID select s).FirstOrDefault();
            if(solution != null)
            {
                submission.Grade = solution.Grade;
            }
            submission.UsersName = (from u in db.Users where u.Id == userID select u.UserName).FirstOrDefault().ToString();
            submission.IsTeacher = isTeacher;
            return View(submission);
        }

        [HttpPost]
        public ActionResult SubmitGrade(SubmissionViewModel submission)
        {
            Solution solution = (from s in db.Solutions where s.SubmissionID == submission.ID select s).FirstOrDefault();
            solution.Grade = submission.Grade;
            db.SaveChanges();
            //await db.SaveChangesAsync();
            int assignmentID = (from m in db.Milestones where m.ID == submission.MilestoneID select m.AssignmentID).FirstOrDefault();
            return RedirectToAction("AllSolutions", new { id = assignmentID});
        }

        public ActionResult RegexTest(SubmissionViewModel submission)
        {
            Regex passPattern = new Regex(submission.Code);
            List<List<string>> excpectedData = sService.GetExpectedRegex(submission.MilestoneID);
            string entityOutput = "Accepted strings:";
            string entityExpected = "Accepted strings:";
            submission.SolutionOutput.Add("Accepted strings:");
            submission.ExpectedOutput.Add("Accepted strings:");
            foreach (var item in excpectedData[0])
            {
                submission.SolutionOutput.Add(item);
                entityOutput += item;
                if (!passPattern.IsMatch(item))
                {
                    submission.ExpectedOutput.Add("Your regex does not accept the string " + item);
                    entityExpected += "Your regex does not accept the string " + item + '\n';
                    submission.IsAccepted = false;
                    Submission newData = new Submission { MilestoneID = submission.MilestoneID, IsAccepted = submission.IsAccepted, SolutionOutput = entityOutput, UserID = User.Identity.GetUserId(), Code = submission.Code, ExpectedOutput = entityExpected};
                    db.Submission.Add(newData);
                    db.SaveChanges();
                    return View(submission);
                }
                submission.ExpectedOutput.Add(item);
                entityExpected += item + '\n';
            }
            submission.SolutionOutput.Add("Not accepted strings:");
            entityOutput += "Not accepted strings: \n";
            foreach (var item in excpectedData[1])
            {
                submission.SolutionOutput.Add(item);
                entityOutput += item + "\n";
                if (passPattern.IsMatch(item))
                {
                    submission.ExpectedOutput.Add("Your regex accepts the string " + item);
                    entityOutput += "Your regex accepts the string " + item + "\n";
                    submission.IsAccepted = false;
                    Submission newData = new Submission { MilestoneID = submission.MilestoneID, IsAccepted = submission.IsAccepted, SolutionOutput = entityExpected, UserID = User.Identity.GetUserId(), Code = submission.Code, ExpectedOutput = entityExpected };
                    db.Submission.Add(newData);
                    return View(submission);
                }
                submission.ExpectedOutput.Add(item);
                entityExpected += item + '\n';
            }
            Submission newSubmission = new Submission { MilestoneID = submission.MilestoneID, IsAccepted = submission.IsAccepted, SolutionOutput = entityExpected, UserID = User.Identity.GetUserId(), Code = submission.Code, ExpectedOutput = entityExpected };
            db.Submission.Add(newSubmission);
            submission.IsAccepted = true;
            return View(submission);
        }

        [ValidateInput(false)]
        public ActionResult Other(SubmissionViewModel submission)
        {
            Submission newSubmission = new Submission { MilestoneID = submission.MilestoneID, IsAccepted = submission.IsAccepted, SolutionOutput = "", UserID = User.Identity.GetUserId(), Code = submission.Code, ExpectedOutput = "" };
            db.Submission.Add(newSubmission);
            return View(submission);
        }

        [ValidateInput(false)]
        public ActionResult OtherWithTests(SubmissionViewModel submission)
        {
            Submission newSubmission = new Submission { MilestoneID = submission.MilestoneID, IsAccepted = submission.IsAccepted, UserID = User.Identity.GetUserId(), Code = submission.Code };
            db.Submission.Add(newSubmission);
            submission.SolutionOutput[0] = submission.Code;
            submission.ExpectedOutput[0] = sService.GetTestCase(submission.MilestoneID);
            return View(submission);
        }

    }

}

