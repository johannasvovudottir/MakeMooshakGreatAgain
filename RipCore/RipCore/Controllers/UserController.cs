using RipCore.Models;
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
        // GET: Student
        public ActionResult Index()
        {
            var viewModels = service.GetAllInfo(1);
            return View(viewModels);
        }

        public ActionResult StudentOverview(int id)
        {
            var viewModel = service.GetCoursesById(id);
            return View(viewModel);
        }

        public ActionResult TeacherOverview(int id)
        {
            var viewModel = service.GetCoursesById(id);
            return View(viewModel);
        }

        public ActionResult Create()
        {
            AssignmentViewModel viewModel = new AssignmentViewModel();
            return View(viewModel);
        }

        public ActionResult Edit()
        {
            AssignmentViewModel viewModel = new AssignmentViewModel();
            return View(viewModel);
        }
    }
}