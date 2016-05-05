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
            var CoursesToAppend = CourseService.GetAllCourses();
            var PersonsToAppend = PersonService.GetAllPersons();

            AdminIndexViewModel viewModel = new AdminIndexViewModel
            {
                Courses = CoursesToAppend,
                Persons = PersonsToAppend
            };

            return View(viewModel);
        }

        public ActionResult AddPerson()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddPerson(PersonViewModel newData)
        {
            //Implement register
            return View();
        }
        public ActionResult EditPerson(string id)
        {
            if (id != null)
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
                ApplicationUser newPerson = db.Users.Where(x => x.Id == newData.ID).SingleOrDefault();
                if (newPerson != null)
                {
                    newPerson.FullName = newData.Name;
                    newPerson.Email = newData.Email;
                    newPerson.PasswordHash = newData.PasswordHash;
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
        [HttpPost]
        public ActionResult AddCourse(AdminCourseOverView newData)
        {
            Course newCourse = new Course { Name = newData.Name, Semester = newData.Semester, Year = newData.Year, SchoolID = 1 };
            db.Courses.Add(newCourse);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult EditCourse(int id)
        {

            if (id != 0)
            {
                AdminCourseOverView viewModel = CourseService.GetCourseByID(id);
                if (viewModel != null)
                {
                    return View(viewModel);
                }
            }
            return View();
        }
        [HttpPost]
        public ActionResult EditCourse(AdminCourseOverView newData)
        {
            if (ModelState.IsValid)
            {
                Course newCourse = db.Courses.Where(x => x.ID == newData.ID).SingleOrDefault();
                if (newCourse != null)
                {
                    newCourse.Name = newData.Name;
                    newCourse.Semester = newData.Semester;
                    newCourse.Year = newData.Year;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View();

        }
        public ActionResult CourseConnections(int courseID)
        {
            if (courseID != 0)
            {
                var PersonsToAppend = PersonService.GetAllPersons();
                var courseToAppend = CourseService.GetCourseByID(courseID).toCourse();
                var teachersToAppend = PersonService.GetAllTeachers(courseID);
                var studentsToAppend = PersonService.GetAllStudents(courseID);
                CourseConnectViewModel viewModel = new CourseConnectViewModel
                {
                    UnConnectedUsers = PersonsToAppend,
                    CurrentCourse = courseToAppend,
                    Teachers = teachersToAppend,
                    Students = studentsToAppend
                };
                return View(viewModel);

            }
            return RedirectToAction("Index");
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

        public ActionResult DeletePerson(string id)
        {
           if (id != null)
            {
                PersonViewModel viewModel = PersonService.GetPersonById(id);
               if (viewModel != null)
                {
                    return View(viewModel);
              }
                
            }
            return View();


           /*PersonViewModel IDFromPersonToDelete = PersonService.GetPersonById(id);
            User userToDelete = new User { ID = IDFromPersonToDelete.ID };
           db.Users.Remove(userToDelete);
            db.SaveChanges();
            return RedirectToAction("Index");*/
        }

        [HttpPost]
        public ActionResult DeletePerson(PersonViewModel newData)
        {
            ApplicationUser userToDelete = db.Users.Where(x => x.Id == newData.ID).SingleOrDefault();
            db.Users.Remove(userToDelete);
            db.SaveChanges();
            return RedirectToAction("Index");

        }

        public ActionResult DeleteCourse(int id)
        {
            if(id != 0)
            {
                AdminCourseOverView viewModel = CourseService.GetCourseByID(id);
                if(viewModel != null)
                {
                    return View(viewModel);
               }
            }
            return View();
        }

        [HttpPost]
        public ActionResult DeleteCourse(AdminCourseOverView newData)
        {
            
                Course courseToDelete = db.Courses.Where(x => x.ID == newData.ID).SingleOrDefault();
                db.Courses.Remove(courseToDelete);
                db.SaveChanges();
                
            
            return RedirectToAction("Index");
        }
        
    }
}