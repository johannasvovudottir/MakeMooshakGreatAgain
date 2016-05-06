using Microsoft.AspNet.Identity;
using RipCore.Models;
using RipCore.Models.Entities;
using RipCore.Models.ViewModels;
using System;
using System.Collections.Generic;
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
            if (viewModel.File != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    viewModel.File.InputStream.CopyTo(memoryStream);

                    string result = Encoding.ASCII.GetString(memoryStream.ToArray());
                    solution.SolutionOutput = result;
                }
            }
            else if(!string.IsNullOrEmpty(viewModel.Solution))
            {
                solution.SolutionOutput = viewModel.Solution;
            }
            //TODO else ef ekkert
            db.Solutions.Add(solution);
            db.SaveChanges();
            return RedirectToAction("StudentOverview", "User", new { id = viewModel.CourseID });
        }
    }
}