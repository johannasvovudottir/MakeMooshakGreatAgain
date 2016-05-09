using Microsoft.AspNet.Identity;
using RipCore.Models;
using RipCore.Models.Entities;
using RipCore.Models.ViewModels;
using RipCore.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RipCore.Controllers
{
    public class UserController : Controller
    {
        private CourseService service = new CourseService();
        private AccountsService accountService = new AccountsService();
        private AssignmentsService assignmentService = new AssignmentsService();
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Student
        public ActionResult Index()
        {
            #region Security
            SecurityRedirect redirect = accountService.VerifySecurityLevel
                (
                    auth:       User.Identity.IsAuthenticated,
                    secLevel:   SecurityState.USER,
                    userID:     User.Identity.GetUserId(),
                    courseID:   null
                );
            if(redirect.Redirect)
            {
                return RedirectToAction(redirect.ActionName, redirect.ControllerName);
            }
            #endregion

            var viewModels = service.GetAllInfo(User.Identity.GetUserId());
            viewModels.Name = User.Identity.Name;
            return View(viewModels);
        }

        public ActionResult StudentOverview(int id)
        {
            #region Security
            SecurityRedirect redirect = accountService.VerifySecurityLevel
                (
                    auth: User.Identity.IsAuthenticated,
                    secLevel: SecurityState.STUDENT,
                    userID: User.Identity.GetUserId(),
                    courseID: id
                );
            if (redirect.Redirect)
            {
                return RedirectToAction(redirect.ActionName, redirect.ControllerName);
            }
            #endregion

            CourseViewModel viewModel = service.GetCoursesById(id, User.Identity.GetUserId());
            viewModel.isTeacher = false;
            return View(viewModel);
        }

        public ActionResult TeacherOverview(int id)
        {
            #region Security
            SecurityRedirect redirect = accountService.VerifySecurityLevel
                (
                    auth: User.Identity.IsAuthenticated,
                    secLevel: SecurityState.TEACHER,
                    userID: User.Identity.GetUserId(),
                    courseID: id
                );
            if (redirect.Redirect)
            {
                return RedirectToAction(redirect.ActionName, redirect.ControllerName);
            }
            #endregion

            CourseViewModel viewModel = service.GetCoursesById(id, User.Identity.GetUserId());
            viewModel.isTeacher = true;
            return View(viewModel);
        }

        public ActionResult Create(int id)
        {
            #region Security
            if(!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            #endregion

            AssignmentViewModel viewModel = new AssignmentViewModel();
            viewModel.CourseID = id;
            viewModel.Milestones = new List<AssignmentMilestoneViewModel>();
            AssignmentMilestoneViewModel milestone = new AssignmentMilestoneViewModel { Title = "", AssignmentID = id };
            viewModel.Milestones.Add(milestone);
            viewModel.programmingLanguages = assignmentService.GetProgrammingLanguages();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(AssignmentViewModel newData, int counter, FormCollection collection, IEnumerable<HttpPostedFileBase> files)
        {
            #region Security
            SecurityRedirect redirect = accountService.VerifySecurityLevel
                (
                    auth: User.Identity.IsAuthenticated,
                    secLevel: SecurityState.TEACHER,
                    userID: User.Identity.GetUserId(),
                    courseID: newData.CourseID
                );
            if (redirect.Redirect)
            {
                return RedirectToAction(redirect.ActionName, redirect.ControllerName);
            }
            #endregion

            int tmp = newData.CourseID;
            if (newData.File != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    newData.File.InputStream.CopyTo(memoryStream);
                    newData.TestCases = Encoding.ASCII.GetString(memoryStream.ToArray());
                }
            }

            Assignment assignemnt = new Assignment { CourseID = newData.CourseID, DateCreated = newData.DateCreated, Description = newData.Description, DueDate = newData.DueDate, TestCases = newData.TestCases, ProgrammingLanguageID = newData.ProgrammingLanguageID, Title = newData.Title, Weight = newData.Weight };
            db.Assignments.Add(assignemnt);
            db.SaveChanges();
            string bla = collection["Milestones[" + 0 + "].ID"];
            for (int i = 0; i < counter; i++)
            {
                //bool exists = collection["Milestones[" + i + "].ID"] != "0";
                //if (!exists)
                //{
                    string title = collection["Milestones[" + i + "].Title"];
                    int weight;
                    Int32.TryParse(collection["Milestones[" + i + "].Weight"], out weight);
                    string description = collection["Milestones[" + i + "].Description"];
                    string testCases = collection["Milestones[" + i + "].TestCases"];
                    int assignmentID = (from a in db.Assignments where a.Title == newData.Title && a.CourseID == newData.CourseID select a.ID).FirstOrDefault();
                    if(assignmentID != 0)
                    {
                        Milestone milestone = new Milestone
                        {
                            Title = title,
                            Weight = weight,
                            Description = description,
                            TestCases = testCases,
                            AssignmentID = assignmentID,
                            DateCreated = newData.DateCreated,
                            DueDate = newData.DueDate,
                            ProgrammingLanguageID = newData.ProgrammingLanguageID
                        };
                        db.Milestones.Add(milestone);
                    }

               //}
            }
            


            return RedirectToAction("TeacherOverview", new { id = newData.CourseID });
        }

        public ActionResult Edit(int id)
        {
            #region Security
            string ID = null;
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            if (!accountService.GetIdByUser(User.Identity.Name, ref ID))
                return RedirectToAction("Index", "Home");

            if (id <= 0)
            {
                return View();
            }
            #endregion
            //if(id.HasValue)
            //{ 
            AssignmentViewModel viewModel = assignmentService.GetAssignmentsById(id);
            if (viewModel != null)
            {
                return View(viewModel);
            }
            //}
            return RedirectToAction("TeacherOverview", new { id = id });
        }

        [HttpPost]
        public ActionResult Edit(AssignmentViewModel model, int counter, FormCollection collection)
        {
            #region Security
            SecurityRedirect redirect = accountService.VerifySecurityLevel
                (
                    auth: User.Identity.IsAuthenticated,
                    secLevel: SecurityState.TEACHER,
                    userID: User.Identity.GetUserId(),
                    courseID: model.CourseID
                );
            if (redirect.Redirect)
            {
                return RedirectToAction(redirect.ActionName, redirect.ControllerName);
            }
            #endregion

            if (ModelState.IsValid)
            {
                for (int i = 0; i < counter; i++)
                {
                    bool exists = collection["Milestones[" + i + "].ID"] != null;
                    if (!exists)
                    {
                        string title = collection["Milestones[" + i + "].Title"];
                        int weight;
                        Int32.TryParse(collection["Milestones[" + i + "].Weight"], out weight);
                        string description = collection["Milestones[" + i + "].Description"];

                        db.Milestones.Add(new Milestone()
                        {
                            Title = title,
                            Weight = weight,
                            Description = description,
                            AssignmentID = model.ID
                        });
                    }
                }

                Assignment assignment = db.Assignments.Where(x => x.ID == model.ID).SingleOrDefault();
                if (assignment != null)
                {
                    assignment.Title = model.Title;
                    assignment.Description = model.Description;
                    assignment.DateCreated = model.DateCreated;
                    assignment.DueDate = model.DueDate;
                    assignment.ProgrammingLanguageID = model.ProgrammingLanguageID;
                    if (model.File != null)
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            model.File.InputStream.CopyTo(memoryStream);
                            assignment.TestCases = Encoding.ASCII.GetString(memoryStream.ToArray());
                            /*string[] result = Encoding.ASCII.GetString(memoryStream.ToArray()).Split(new string[] { Environment.NewLine "QUIT" }, StringSplitOptions.None);
                            for(int i = 0; i < result.Length; i=i+2)
                            {
                                assignment.Input = result[i];
                                assignment.Output = result[i + 1];
                            }*/
                        }
                    }
                    db.SaveChanges();
                }
                return RedirectToAction("Index");

            }
            return View(model);
        }

        public ActionResult StudentAssignmentView(int id)
        {
            #region Security
            string userID = null;
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (!accountService.GetIdByUser(User.Identity.Name, ref userID))
                return RedirectToAction("Index", "Home");

            //if (!accountService.IsUserQualified("Teacher", userID, id) || !accountService.IsUserQualified("Student", userID, id))
              //  return RedirectToAction("Index", "User");


            if (id <= 0)
                return View();
            #endregion
            AssignmentViewModel viewModel = assignmentService.GetAssignmentForView(id, false);
            return View(viewModel);
        }

        public ActionResult TeacherAssignmentView(int id)
        {
            #region Security
            string userID = null;
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (!accountService.GetIdByUser(User.Identity.Name, ref userID))
                return RedirectToAction("Index", "Home");

            //if (!accountService.IsUserQualified("Teacher", userID, id))
              //  return RedirectToAction("Index", "User");

            if (id <= 0)
                return View();
            #endregion

            AssignmentViewModel viewModel = assignmentService.GetAssignmentForView(id, true);
            viewModel.UserID = User.Identity.GetUserId();
            return View(viewModel);
        }

        public ActionResult AddMilestone(FormCollection collection, HttpPostedFileBase files)
        {
            var test = collection["files"];
            var test2 = Request.Files["files"];
            var test3 = files;
            string TestCases;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                files.InputStream.CopyTo(memoryStream);
                TestCases = Encoding.ASCII.GetString(memoryStream.ToArray());
            }

            if (String.IsNullOrEmpty(TestCases))
            {
                return Json("", JsonRequestBehavior.AllowGet);
                //return RedirectToAction("IndexJSON", "MovieApp", new { id = movieId });
            }
            return Json(TestCases, JsonRequestBehavior.AllowGet);
        }

    }
}