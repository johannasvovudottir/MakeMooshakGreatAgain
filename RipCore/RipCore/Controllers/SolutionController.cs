﻿using Microsoft.AspNet.Identity;
using RipCore.Models;
using RipCore.Models.Entities;
using RipCore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RipCore.Controllers
{
    public class SolutionController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Solution
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SubmitSolution(AssignmentViewModel viewModel)
        {
            Solution solution = new Solution { AssignmentID = viewModel.ID, StudentID = User.Identity.GetUserId() };
            SubmissionViewModel submission = new SubmissionViewModel { AssignmentName = viewModel.Title, AssignmentID = viewModel.ID };
            if (viewModel.File != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    viewModel.File.InputStream.CopyTo(memoryStream);

                    string result = Encoding.ASCII.GetString(memoryStream.ToArray());
                    submission.SolutionOutput = result;
                }
            }

            else if(!string.IsNullOrEmpty(viewModel.Solution))
            {
                 submission.SolutionOutput = viewModel.Solution;
            }
            
            //TODO else ef ekkert
<<<<<<< HEAD
            db.Solutions.Add(solution);
            db.SaveChanges();
<<<<<<< HEAD
            return RedirectToAction("StudentOverview", "User", new { id = viewModel.CourseID });
=======

            CompileSolution(solution.SolutionOutput);

            return RedirectToAction("StudentOverview", "User", new { id = viewModel.CourseID });
        }

        ///[HttpPost] // ??
        public ActionResult CompileSolution(string data)
=======

            return RedirectToAction("CompileSolution", submission);

        }

        ///[HttpPost] // ??
        public ActionResult CompileSolution(Submission data)
>>>>>>> johanna
        {
            // To simplify matters, we declare the code here.
            // The code would of course come from the student!
            var code = "#include <iostream>\n" +
                    "using namespace std;\n" +
                    "int main()\n" +
                    "{\n" +
                    "cout << \"Hello world\" << endl;\n" +
                    "cout << \"The output should contain two lines\" << endl;\n" +
                    "return 0;\n" +
                    "}";

            // Set up our working folder, and the file names/paths.
            // In this example, this is all hardcoded, but in a
            // real life scenario, there should probably be individual
            // folders for each user/assignment/milestone.
            var workingFolder = "C:\\Temp\\Mooshak2Code\\";
            var cppFileName = "Hello.cpp";
            var exeFilePath = workingFolder + "Hello.exe";

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
            compiler.WaitForExit();
            compiler.Close();

            // Check if the compile succeeded, and if it did,
            // we try to execute the code:
            if (System.IO.File.Exists(exeFilePath))
            {
                var processInfoExe = new ProcessStartInfo(exeFilePath, "");
                processInfoExe.UseShellExecute = false;
                processInfoExe.RedirectStandardOutput = true;
                processInfoExe.RedirectStandardError = true;
                processInfoExe.CreateNoWindow = true;
                using (var processExe = new Process())
                {
                    processExe.StartInfo = processInfoExe;
                    processExe.Start();
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
                }
            }

            // TODO: We might want to clean up after the process, there
            // may be files we should delete etc.

            return View();
<<<<<<< HEAD
>>>>>>> 52d0037cdc4ddbdaf4ff125fbb8b03a1ecb9d7cd
=======
>>>>>>> johanna
        }
    }
}