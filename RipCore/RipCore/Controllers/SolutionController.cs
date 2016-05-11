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
            SubmissionViewModel submission = new SubmissionViewModel { AssignmentName = viewModel.Title, MilestoneID = viewModel.milestoneSubmissionID };
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
            submission.ProgrammingLanguage = "html";
            if (submission.ProgrammingLanguage == "regex")
            {
                return RedirectToAction("RegexTest", submission);
            }
            if (submission.ProgrammingLanguage == "html")
            {
                return RedirectToAction("HtmlCode", submission);
            }
            return RedirectToAction("CompileSolution", submission);
        }

        ///[HttpPost] // ??
        [ValidateInput(false)]
        public ActionResult CompileSolution(SubmissionViewModel data)
        {
            // To simplify matters, we declare the code here.
            // The code would of course come from the student!

            var code = data.Code;

            // Set up our working folder, and the file names/paths.
            // In this example, this is all hardcoded, but in a
            // real life scenario, there should probably be individual
            // folders for each user/assignment/milestone.
            string user = User.Identity.GetUserId();

            string smu = AppDomain.CurrentDomain.BaseDirectory;
            //string smusmu = System.IO.Directory.GetCurrentDirectory();
            //string smusmusmu = Environment.CurrentDirectory;
            //string smusmusmusmu = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            //string smusmusmusmusmusmu = System.IO.Path.GetDirectoryName(
            //System.Reflection.Assembly.GetExecutingAssembly().Location);

            var workingFolder = smu + user + "\\"; //name; // eða ID
            System.IO.Directory.CreateDirectory(workingFolder);

            var cppFileName = data.AssignmentName.Replace(" ", "").ToLower() + data.ProgrammingLanguage; // ---- Verkefnaheiti
            var exeFilePath = workingFolder + data.AssignmentName.Replace(" ", "").ToLower() + ".exe"; // ----- verkefnaheiti

            // Write the code to a file, such that the compiler
            // can find it:
            System.IO.File.WriteAllText(workingFolder + cppFileName, code);

            // In this case, we use the C++ compiler (cl.exe) which ships
            // with Visual Studio. It is located in this folder:
            var compilerFolder = "C:\\Program Files (x86)\\Microsoft Visual Studio 14.0\\VC\\bin\\";
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
            var compilerTest = compiler.WaitForExit(10000);
            if (!compilerTest)
            {
                data.SolutionOutput = "Compile Time Error!";
                compiler.Kill();
                return View(data);
            }
            compiler.Close();


            // Check if the compile succeeded, and if it did,
            // we try to execute the code:
            if (System.IO.File.Exists(exeFilePath))
            {
                data.SolutionOutput = "";
                var processInfoExe = new ProcessStartInfo(exeFilePath, "");
                processInfoExe.UseShellExecute = false;
                processInfoExe.RedirectStandardOutput = true;
                processInfoExe.RedirectStandardInput = true;
                processInfoExe.RedirectStandardError = true;
                processInfoExe.CreateNoWindow = true;
                List<Tuple<string, string>> excpectedData = sService.GetExpectedData(data.MilestoneID);
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
                            data.SolutionOutput = "Run Time Error!";
                            processExe.Kill();
                            return View(data);
                        }

                        // We then read the output of the program:
                        var lines = new List<string>();
                        while (!processExe.StandardOutput.EndOfStream)
                        {
                            lines.Add(processExe.StandardOutput.ReadLine());
                        }


                        ViewBag.Output = lines;
                        data.SolutionOutput += lines[0] + '\n';
                        data.ExpectedOutput += excpectedData[i].Item2 + '\n';
                        if (string.Equals(lines[0].ToString(), excpectedData[i].Item2))
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

                Submission submission = new Submission { MilestoneID = data.MilestoneID, IsAccepted = data.IsAccepted, SolutionOutput = data.SolutionOutput, UserID = User.Identity.GetUserId(), Code = data.Code, ExpectedOutput= data.ExpectedOutput };
                db.Submission.Add(submission);
                db.SaveChanges();

                Solution solution = (from s in db.Solutions where s.MilestoneID == submission.MilestoneID && s.StudentID == submission.UserID select s).FirstOrDefault();
                if(solution == null)
                {
                    Solution soliholm = new Solution { MilestoneID = submission.MilestoneID, StudentID = submission.UserID, SubmissionID = submission.ID, Code = data.Code }; //sService.GetBestSubmissionByID(data.MilestoneID, User.Identity.GetUserId());
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
                Submission submission = new Submission { MilestoneID = data.MilestoneID, IsAccepted = data.IsAccepted, SolutionOutput = data.SolutionOutput, UserID = User.Identity.GetUserId(), Code = data.Code, ExpectedOutput = data.ExpectedOutput };
                db.Submission.Add(submission);
                db.SaveChanges();
                data.SolutionOutput = "Please add an executable file!";
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
            return RedirectToAction("AllSolutions", assignmentID);
        }

        public ActionResult RegexTest(SubmissionViewModel submission)
        {
            Regex passPattern = new Regex(submission.Code);
            List<List<string>> excpectedData = sService.GetExpectedRegex(submission.MilestoneID);
            submission.SolutionOutput = "Accepted strings:\n";
            submission.ExpectedOutput = "Accepted strings:\n";
            foreach (var item in excpectedData[0])
            {
                submission.SolutionOutput += item + '\n';
                if (!passPattern.IsMatch(item))
                {
                    submission.ExpectedOutput += "Your regex does not accept the string " + item;
                    submission.IsAccepted = false;
                    return View(submission);
                }
                submission.ExpectedOutput += item + '\n';
            }
            submission.SolutionOutput += "Not accepted strings:\n";
            foreach (var item in excpectedData[1])
            {
                submission.SolutionOutput += item + '\n';
                if (passPattern.IsMatch(item))
                {
                    submission.ExpectedOutput += "Your regex accepts the string " + item;
                    submission.IsAccepted = false;
                    return View(submission);
                }
                submission.ExpectedOutput += item + '\n';
            }
            submission.IsAccepted = true;
            return View(submission);
        }

        [ValidateInput(false)]
        public ActionResult HtmlCode(SubmissionViewModel submission)
        {
            return View(submission);
        }

    }

}

