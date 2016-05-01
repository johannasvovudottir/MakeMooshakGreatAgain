using RipCore.Models;
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
            var viewModels = service.GetCoursesWhereStudent(2);
            return View(viewModels);
        }

        public ActionResult CourseDetails(int id)
        {
            var viewModel = service.GetCoursesById(id);
            return View(viewModel);
        }
    }
}