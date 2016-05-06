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
            string ID = null;
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (!accountService.GetIdByUser(User.Identity.Name, ref ID))
                return RedirectToAction("Index", "Home");
            #endregion

            var viewModels = service.GetAllInfo(ID);
            viewModels.Name = User.Identity.Name;
            return View(viewModels);
        }

        public ActionResult StudentOverview(int id)
        {
            #region Security
            string actualID = null;
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (!accountService.GetIdByUser(User.Identity.Name, ref actualID))
                return RedirectToAction("Index", "Home");

            if (!accountService.IsUserQualified("Teacher", actualID, id))
            {
                if (!accountService.IsUserQualified("Student", actualID, id))
                {
                   // return RedirectToAction("Index", "User");
                }
            }
            #endregion

            CourseViewModel viewModel = service.GetCoursesById(id, actualID);
            viewModel.isTeacher = false;
            return View(viewModel);
        }

        public ActionResult TeacherOverview(int id)
        {
            #region Security
            string actualID = null;
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (!accountService.GetIdByUser(User.Identity.Name, ref actualID))
                return RedirectToAction("Index", "Home");

            if (!accountService.IsUserQualified("Teacher", actualID, id))
            {
                return RedirectToAction("Index", "User");
            }
            #endregion

            CourseViewModel viewModel = service.GetCoursesById(id, actualID);
            viewModel.isTeacher = true;
            return View(viewModel);
        }

        public ActionResult Create(int id)
        {
            #region Security
            string ID = null;
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            if (!accountService.GetIdByUser(User.Identity.Name, ref ID))
                return RedirectToAction("Index", "Home");
            #endregion

            AssignmentViewModel viewModel = new AssignmentViewModel();
            viewModel.CourseID = id;
            viewModel.Milestones = new List<AssignmentMilestoneViewModel>();
            viewModel.programmingLanguages = assignmentService.GetProgrammingLanguages();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(AssignmentViewModel newData)
        {
            #region Security
            string ID = null;
            accountService.GetIdByUser(User.Identity.Name, ref ID);
            if (!User.Identity.IsAuthenticated)
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "User");
            }

            if(!accountService.IsUserQualified("Teacher", ID, newData.CourseID))
            {
                return RedirectToAction("Index", "User");
            }
            #endregion

            int tmp = newData.CourseID;
            Assignment newAssignment = new Assignment { Title = newData.Title, CourseID = newData.CourseID, Weight = newData.Weight, DueDate = newData.DueDate, DateCreated = newData.DateCreated, Description = newData.Description, Input = newData.Input, Output = newData.Output, ProgrammingLanguageID = newData.ProgrammingLanguageID };
            UpdateModel(newAssignment);
            db.Assignments.Add(newAssignment);
            db.SaveChanges();
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
            string ID = null;
            if (User.Identity.IsAuthenticated)
            {
                accountService.GetIdByUser(User.Identity.Name, ref ID);
            }
            if (!accountService.IsUserQualified("Teacher", ID, model.CourseID))
            {
                return RedirectToAction("Index", "User");
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

                            string[] result = Encoding.ASCII.GetString(memoryStream.ToArray()).Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                            assignment.Input = result[0];
                            assignment.Output = result[1];
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

    }
}