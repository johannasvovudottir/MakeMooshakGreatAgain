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
        private AssignmentsService assignmentService = new AssignmentsService();
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Student
        public ActionResult Index()
        {
            var viewModels = service.GetAllInfo(1);
            return View(viewModels);
        }

        public ActionResult StudentOverview(int id, int userID)
        {
            var viewModel = service.GetCoursesById(id, userID);
            viewModel.isTeacher = false;
            return View(viewModel);
        }

        public ActionResult TeacherOverview(int id, int userID)
        {
            var viewModel = service.GetCoursesById(id, userID);
            viewModel.isTeacher = true;
            return View(viewModel);
        }

        public ActionResult Create(int id)
        {
            return View(new AssignmentViewModel { CourseID = id });
        }

        [HttpPost]
        public ActionResult Create(AssignmentViewModel newData)
        {
            Assignment newAssignment = new Assignment { Title = newData.Title, CourseID = 1, Weight = newData.Weight, DueDate = newData.DueDate, DateCreated = newData.DateCreated, Description = newData.Description };
            UpdateModel(newAssignment);
            db.Assignments.Add(newAssignment);
            db.SaveChanges();
            return RedirectToAction("TeacherOverview", new { id=1, userID = 1});
        }

        public ActionResult Edit(int id)
        {
            //if(id.HasValue)
            //{
                AssignmentViewModel viewModel = assignmentService.GetAssignmentsById(id);
                if(viewModel != null)
                {
                    return View(viewModel);
                }
            //}
            return RedirectToAction("TeacherOverview", new { id = 1, userID = 1 });
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