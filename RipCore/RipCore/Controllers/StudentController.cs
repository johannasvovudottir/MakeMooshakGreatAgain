using RipCore.Models;
using RipCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RipCore.Controllers
{
    public class StudentController : Controller
    {
        private UsersService service = new UsersService();
        // GET: Student
        public ActionResult Index()
        {
            int id = 1;
            var viewModel = service.GetCoursesById(id);
            return View(viewModel);
        }
    }
}