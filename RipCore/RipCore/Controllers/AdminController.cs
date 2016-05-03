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
    public class AdminController : Controller
    {
        private CourseService CourseService = new CourseService();
        private PersonService PersonService = new PersonService();
        private AccountsService accountService = new AccountsService();
        private ApplicationDbContext db = new ApplicationDbContext();
        private EncryptionService encService = new EncryptionService();
        // GET: Admin
        public ActionResult Index()
        {
            //int id = accountService.GetIdByUser(User.Identity.Name);

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
        [HttpPost]
        public ActionResult AddPerson(PersonViewModel newData)
        {
            string key = encService.RandomizePasskey();
            string encrypted = encService.Encrypt(newData.Password, key);
            User newPerson = new User { FullName = newData.Name, Ssn = newData.Ssn,UserName=newData.Username,Email=newData.Email,Password=encrypted,Passkey=key};
            db.Users.Add(newPerson);
            db.SaveChanges();
            return View();
        }
        public ActionResult EditPerson(int id)
        {
            if(id != 0)
            {

            PersonViewModel viewModel = PersonService.GetPersonById(id);
                if (viewModel != null)
                {
                    return View(viewModel);
                }
            }
            return View();
        }
        [HttpPost]
        public ActionResult EditPerson(PersonViewModel newData)
        {
            if (ModelState.IsValid)
            {
                User newPerson = db.Users.Where(x => x.ID == newData.ID).SingleOrDefault();
                if (newPerson != null)
                {
                    newPerson.FullName = newData.Name;
                    newPerson.Email = newData.Email;
                    newPerson.Password = newData.Password;
                    newPerson.Passkey = newData.Passkey;
                    newPerson.Ssn = newData.Ssn;
                    newPerson.UserName = newData.Username;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
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