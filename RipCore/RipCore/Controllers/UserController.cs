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
                return RedirectToAction("Index", "Home");

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

            bool isQualified = accountService.IsUserQualified("Teacher", actualID, id);
            if (userID != actualID)
            {
                var model = service.GetCoursesById(id, actualID);
                model.UserID = actualID;
                model.isTeacher = isQualified;
                return View(model);
            }
            #endregion

            var viewModel = service.GetCoursesById(id, userID);
            viewModel.isTeacher = isQualified;
            return View(viewModel);
        }

        public ActionResult Create(int id)
        {
            /*
            int userID = accountService.GetIdByUser(User.Identity.Name);
            if (!accountService.IsUserQualified("Teacher", userID, id))
            {
                return RedirectToAction("", "User");
            }
            */
            AssignmentViewModel viewModel = new AssignmentViewModel();
            viewModel.CourseID = id;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(AssignmentViewModel newData)
        {
            #region Security
            int ID = 0;
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            if (!accountService.GetIdByUser(User.Identity.Name, ref ID))
                return RedirectToAction("Index", "Home");
            #endregion

            int tmp = newData.CourseID;
            Assignment newAssignment = new Assignment { Title = newData.Title, CourseID = newData.CourseID, Weight = newData.Weight, DueDate = newData.DueDate, DateCreated = newData.DateCreated, Description = newData.Description };
            UpdateModel(newAssignment);
            db.Assignments.Add(newAssignment);
            db.SaveChanges();

            return RedirectToAction("TeacherOverview", new { id=newData.CourseID, userID = ID});
        }

        public ActionResult Edit(int assignID)
        {
            #region Security
            int ID = 0;
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            if (!accountService.GetIdByUser(User.Identity.Name, ref ID))
                return RedirectToAction("Index", "Home");

            if(assignID <= 0)
            {
                return View();
            }
            #endregion
            //if(id.HasValue)
            //{
            AssignmentViewModel viewModel = assignmentService.GetAssignmentsById(assignID);
                if(viewModel != null)
                {
                    return View(viewModel);
                }
            //}
            return RedirectToAction("TeacherOverview", new { id = assignID, userID = ID });
        }

        [HttpPost]
        public ActionResult Edit(AssignmentViewModel model)
        {
            if (ModelState.IsValid)
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
            return View(model);
        }
    }
}