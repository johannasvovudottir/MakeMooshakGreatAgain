﻿using RipCore.Models.ViewModels;
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
        private CourseService CourseService = new CourseService();
        private PersonService PersonService = new PersonService();
        private AccountsService accountService = new AccountsService();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        //public ActionResult User()
        //{
        //    return View();
        //}
        public ActionResult AddPerson()
        {
            return View();
        }
        public ActionResult EditPerson(int id)
        {
            if(id != null)
            {

            PersonViewModel viewModel = PersonService.GetPersonById(id);
                if (viewModel != null)
                {
                    return View(viewModel);
                }
            }
            // return RedirectToAction("TeacherOverview", new { id = 1, userID = 1 });
            return View();

        }
        public ActionResult AddCourse()
        {
            return View();
        }
        public ActionResult EditCourse()
        {
            return View();
        }
        public ActionResult CourseOverview()
        {
            var viewModel = CourseService.GetAllCourses();
            return View(viewModel);
        }
        public ActionResult PersonOverview()
        {
            var viewModel = PersonService.GetAllPersons();
            return View(viewModel);
        }
        
    }
}