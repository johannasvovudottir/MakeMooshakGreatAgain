using RipCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RipCore.Controllers
{
    public class AdminController : Controller
    {
        private CourseService service = new CourseService();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult User()
        {
            return View();
        }
        public ActionResult CourseOverview()
        {
            var viewModel = service.GetCoursesWhereStudent(2);
            return View(viewModel);
        }
        public ActionResult PersonOverview()
        {
            return View();
        }

    }
}