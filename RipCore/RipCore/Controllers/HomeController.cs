using Microsoft.AspNet.Identity;
using RipCore.Models;
using RipCore.Models.Entities;
using RipCore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RipCore.Controllers
{
    public class HomeController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            bool isLoggedIn = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            HomeIndexViewModel viewModel = new HomeIndexViewModel { IsLoggedIn = isLoggedIn };
            if (isLoggedIn)
            {
                string userName = User.Identity.GetUserName();
                viewModel.UserName = userName;
            }           
            return View(viewModel);
        }

        public ActionResult About()
        {
            AssignmentViewModel tmp = new AssignmentViewModel { ID = 1, CourseID = 1, CourseName = "Test", DateCreated = DateTime.Now, Description = "Eg er litil verkefnalysing", DueDate = DateTime.Now, IsTeacher = false, Title = "eg er litill titill", Milestones = new List<AssignmentMilestoneViewModel>() };
            tmp.milestoneNumber = GetMilestonesNumber(66);
            return View(tmp);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public List<SelectListItem> GetMilestonesNumber(int assignmentID)
        {
            List<SelectListItem> milestonesNumber = new List<SelectListItem>();
            List<Milestone> milestones = (from m in db.Milestones where m.AssignmentID == assignmentID select m).ToList();
            for (int i = 0; i < milestones.Count; i++)
            {
                string value = milestones[i].ID.ToString();
                milestonesNumber.Add(new SelectListItem() { Value = value, Text = milestones[i].Title });
            }
            return milestonesNumber;
        }
    }
}