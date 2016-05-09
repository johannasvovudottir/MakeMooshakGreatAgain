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
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SubmitSolution(AssignmentViewModel viewModel)
        {
            SubmissionViewModel submission = new SubmissionViewModel { AssignmentName = viewModel.Title, MilestoneID = viewModel.milestoneSubmissionID };
            if (viewModel.File != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    viewModel.File.InputStream.CopyTo(memoryStream);

                    string result = Encoding.ASCII.GetString(memoryStream.ToArray());
                    submission.SolutionOutput = result;
                }
            }
            else if (!string.IsNullOrEmpty(viewModel.Solution))
            {
                submission.SolutionOutput = viewModel.Solution;
            }

            return RedirectToAction("CompileSolution", submission);
        }

        ///[HttpPost] // ??
        [ValidateInput(false)]
        public ActionResult CompileSolution(SubmissionViewModel data)
        {
            // To simplify matters, we declare the code here.
            // The code would of course come from the student!

            var code = data.SolutionOutput;

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

            var cppFileName = data.AssignmentName.Replace(" ", "").ToLower() + ".cpp"; // ---- Verkefnaheiti
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
            compiler.WaitForExit(10000); // <----- Setja tölu hér inn.. ----
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
                List<Tuple<string, string>> excpectedData = new List<Tuple<string, string>>(); // = sService.GetExpectedData(data.MilestoneID);
                using (var processExe = new Process())
                {
                    processExe.StartInfo = processInfoExe;
                    processExe.Start();
                    //Task.Factory.StartNew(() => { Thread.Sleep(10000); processExe.Kill(); });
                    var test = processExe.WaitForExit(30000);
                    if(!test)
                    {
                        data.SolutionOutput = "Compile Time Error!";
                        processExe.Kill();
                        return View(data);
                    }
                    for (int i = 0; i < excpectedData.Count; i++)
                        {

                        if (excpectedData[i].Item1 != "")
                        {
                            processExe.StandardInput.WriteLine(excpectedData[i].Item1);
                        }
                        // In this example, we don't try to pass any input
                        // to the program, but that is of course also
                        // necessary. We would do that here, using
                        // processExe.StandardInput.WriteLine(), similar
                        // to above.

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
                            Solution solution = new Solution { MilestoneID = data.MilestoneID, StudentID = User.Identity.GetUserId() };
                            db.Solutions.Add(solution);
                            db.SaveChanges();
                        }
                        else
                        {
                            data.IsAccepted = false;
                            break;
                        }
                    }
                }
                Submission submission = new Submission { MilestoneID = data.MilestoneID, IsAccepted = data.IsAccepted, SolutionOutput = data.SolutionOutput, UserID = User.Identity.GetUserId() };
                db.Submission.Add(submission);
                db.SaveChanges();
                // ------ solutionOutput er allt sem er í skjalinu. ------
        }

            // TODO: We might want to clean up after the process, there
            // may be files we should delete etc.

            Directory.Delete(workingFolder, true); //Deletar moppunni sem vid gerdum adan

            return View(data);
        }

        public ActionResult AllSubmissions(int id)
        {
            SubmissionsOverViewModel submissions = new SubmissionsOverViewModel { submissions = sService.GetAllSubmissions(id) };
            return View(submissions);
        }

        public ActionResult SubmissionDetails(int id)
        {
            SubmissionsOverViewModel submissions = new SubmissionsOverViewModel { submissions = sService.GetAllSubmissions(id) };
            return View(submissions);
        }

    }
    
}

