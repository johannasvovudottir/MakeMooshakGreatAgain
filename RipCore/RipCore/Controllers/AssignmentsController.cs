using RipCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RipCore.Controllers
{
    public class AssignmentsController : Controller
    {
        // GET: Assignments
        private AssignmentsService service = new AssignmentsService();

        // GET: /<controller>/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(int id)
        {
            var viewModel = service.GetAssignmentsById(id);
            return View(viewModel);
        }
}
}