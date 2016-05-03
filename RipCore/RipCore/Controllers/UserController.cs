using RipCore.Models;
using RipCore.Models.Entities;
using RipCore.Models.ViewModels;
using RipCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
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
            int ID = 0;
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (!accountService.GetIdByUser(User.Identity.Name, ref ID))
                return RedirectToAction("Index", "Home");
            #endregion

            var viewModels = service.GetAllInfo(ID);
            viewModels.Name = User.Identity.Name;
            return View(viewModels);
        }

        public ActionResult StudentOverview(int id, int userID)
        {
            #region Security
            int actualID = 0;
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (!accountService.GetIdByUser(User.Identity.Name, ref actualID))
                return RedirectToAction("Index", "Home");

            if(!accountService.IsUserQualified("Teacher", actualID, id));
            {
                if (!accountService.IsUserQualified("Student", actualID, id))
                {
                    return RedirectToAction("Index", "User");
                }
            }
            if (userID != actualID)
            {
                var model = service.GetCoursesById(id, actualID);
                model.UserID = actualID;
                model.isTeacher = false;
                return View(model);
            }
            #endregion

            var viewModel = service.GetCoursesById(id, userID);
            viewModel.isTeacher = false;
            return View(viewModel);
        }

        public ActionResult TeacherOverview(int id, int userID)
        {
            #region Security
            int actualID = 0;
            if(!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (!accountService.GetIdByUser(User.Identity.Name, ref actualID))
                return RedirectToAction("Index", "Home");

            if (!accountService.IsUserQualified("Teacher", actualID, id)) 
            {
                return RedirectToAction("Index", "User");
            }

            if (userID != actualID)
            {
                var model = service.GetCoursesById(id, actualID);
                model.UserID = actualID;
                model.isTeacher = true;
                return View(model);
            }
            #endregion

            var viewModel = service.GetCoursesById(id, userID);
            viewModel.isTeacher = true;
            return View(viewModel);
        }

        public ActionResult Create(int id)
        {
            #region Security
            int ID = 0;
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            if (!accountService.GetIdByUser(User.Identity.Name, ref ID))
                return RedirectToAction("Index", "Home");
            #endregion

            AssignmentViewModel viewModel = new AssignmentViewModel();
            viewModel.CourseID = id;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(AssignmentViewModel newData)
        {
            #region Security
            int ID = 0;
            if(User.Identity.IsAuthenticated)
            {
                accountService.GetIdByUser(User.Identity.Name, ref ID);
            }
            if(!accountService.IsUserQualified("Teacher", ID, newData.CourseID))
            {
                return RedirectToAction("Index", "User");
            }
            #endregion

            int tmp = newData.CourseID;
            Assignment newAssignment = new Assignment { Title = newData.Title, CourseID = newData.CourseID, Weight = newData.Weight, DueDate = newData.DueDate, DateCreated = newData.DateCreated, Description = newData.Description };
            UpdateModel(newAssignment);
            db.Assignments.Add(newAssignment);
            db.SaveChanges();
            return RedirectToAction("TeacherOverview", new { id=newData.CourseID, userID = ID});
        }

        public ActionResult Edit(int id)
        {
            #region Security
            int ID = 0;
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            if (!accountService.GetIdByUser(User.Identity.Name, ref ID))
                return RedirectToAction("Index", "Home");

            if(id <= 0)
            {
                return View();
            }
            #endregion
            //if(id.HasValue)
            //{
            AssignmentViewModel viewModel = assignmentService.GetAssignmentsById(id);
            viewModel.CurrentMilestone = new AssignmentMilestoneViewModel();
                if(viewModel != null)
                {
                    return View(viewModel);
                }
            //}
            return RedirectToAction("TeacherOverview", new { id = id, userID = ID });
        }

        [HttpPost]
        public ActionResult Edit(AssignmentViewModel model)
        {
            #region Security
            int ID = 0;
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
                if(model.CurrentMilestone != null)
                {
                    Milestone milestone = new Milestone { Title = model.CurrentMilestone.Title, Description = model.CurrentMilestone.Description, Weight = model.CurrentMilestone.Weight, AssignmentID = model.CurrentMilestone.AssignmentID };
                    UpdateModel(milestone);
                    db.Milestones.Add(milestone);
                    db.SaveChanges();
                    AssignmentViewModel viewModel = assignmentService.GetAssignmentsById(model.ID);
                    viewModel.CurrentMilestone = new AssignmentMilestoneViewModel();
                    return RedirectToAction("Edit", new { id = model.ID });
                }

                else
                {
                    Assignment assignment = db.Assignments.Where(x => x.ID == model.ID).SingleOrDefault();
                    if (assignment != null)
                    {
                        assignment.Title = model.Title;
                        assignment.Description = model.Description;
                        assignment.DateCreated = model.DateCreated;
                        assignment.DueDate = model.DueDate;
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }

            }
            return View(model);
        }
        
        public ActionResult StudentAssignmentView(int id, int userID)
        {
            AssignmentViewModel viewModel = assignmentService.GetAssignmentForView(id, false);
            return View(viewModel);
        }

        public ActionResult TeacherAssignmentView(int id, int userID)
        {
            AssignmentViewModel viewModel = assignmentService.GetAssignmentForView(id, true);
            return View(viewModel);
        }

        public ActionResult AddMilestone(int id)
        {
            AssignmentViewModel viewModel = assignmentService.GetAssignmentForView(id, true);
            return View(viewModel);
        }
    }
}