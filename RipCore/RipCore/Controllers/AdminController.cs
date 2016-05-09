using RipCore.Models;
using RipCore.Models.Entities;
using RipCore.Models.ViewModels;
using RipCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace RipCore.Controllers
{
    public class AdminController : BaseController
    {
        private CourseService CourseService = new CourseService();
        private PersonService PersonService = new PersonService();
        private AccountsService accountService = new AccountsService();
        private ApplicationDbContext db = new ApplicationDbContext();
        private EncryptionService encService = new EncryptionService();
        private ApplicationUserManager _userManager;
        // GET: Admin

        public AdminController()
        {

        }

        public AdminController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

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
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPerson(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, FullName = model.FullName, Ssn = model.SSN };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var res = await UserManager.UpdateAsync(user);
                    return RedirectToAction("Index", "Admin");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult EditPerson(string id)
        {
           
            if (id != null)
            {
                var user = PersonService.GetPersonById(id);
                bool Admin = PersonService.checkIfAdmin(id);
                RegisterViewModel viewModel = new RegisterViewModel {
                    ID = id,
                    UserName = user.Username,
                    FullName = user.Name,
                    Email = user.Email,
                    SSN = user.Ssn,
                    isAdmin = Admin
                };
                if (viewModel != null)
                {
                return View(viewModel);
                }
            }   
            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPerson(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(model.ID);
                user.Email = model.Email;
                user.FullName = model.FullName;
                user.UserName = model.UserName;
                user.Ssn = model.SSN;
                var result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    db.SaveChanges();
                    await db.SaveChangesAsync();
                }
                return RedirectToAction("Index", "Admin");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
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

        public ActionResult EditCourse(int? id)
        {
            
            if (id.HasValue)
            {
                int ID = id.Value;
            
                AdminCourseOverView viewModel = CourseService.GetCourseByID(ID);
                if (viewModel != null)
                {
                    return View(viewModel);
                }
            }
            return RedirectToAction("Index");
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

        public ActionResult ChangeAdminStatus(string userID, bool isAdmin)
        {
            if (isAdmin == true)
            {
                PersonService.removeAdmin(userID);
            }
            else
            {
                PersonService.makeAdmin(userID);
            }


            return RedirectToAction("EditPerson", new { id = userID });
        }

        public ActionResult CourseConnections(int? courseID)
        {
            if (courseID.HasValue)
            {
                int ID = courseID.Value;
                //var PersonsToAppend = PersonService.GetAllPersons();
                var PersonsToAppend = PersonService.GetAllNotConnected(ID);
                var courseToAppend = CourseService.GetCourseByID(ID).toCourse();
                var teachersToAppend = PersonService.GetAllTeachers(ID);
                var studentsToAppend = PersonService.GetAllStudents(ID);
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

        [HttpPost]
        public ActionResult CourseConnections(CourseConnectViewModel newData)
        {
            List<PersonViewModel> persons = newData.UnConnectedUsers;
            //gera selectorFor a nytt eigindi i personview sem heitir isTeacher
            //gera sidan if setningu i lykkjuna og nytt query

            foreach (PersonViewModel item in persons) {
                if (item.isChecked == true) {
                    if (item.Role == "Student")
                    {
                        PersonService.ConnectStudents(item.ID, (newData.CurrentCourse.ID));
                    }
                    else if (item.Role == "Teacher")
                    {
                        PersonService.ConnectTeachers(item.ID, (newData.CurrentCourse.ID));
                    }
                }
            }

            return RedirectToAction("CourseConnections", new { courseID = newData.CurrentCourse.ID });
        }

        public ActionResult RemoveFromCourse(string ID , int courseID, string role)
        {
            if (role == "Teacher")
            {
                Course_Teacher teacherToDelete = (from x in db.CoursesTeachers
                                                  where x.TeacherID == ID &&
                                                  x.CourseID == courseID
                                                  select x).SingleOrDefault();
                db.CoursesTeachers.Remove(teacherToDelete);
                db.SaveChanges();
            }
            else if (role == "Student")
            {
                Course_Student studentToDelete = (from x in db.CoursesStudents
                                                  where x.UserID == ID &&
                                                  x.CourseID == courseID
                                                  select x).SingleOrDefault();
                db.CoursesStudents.Remove(studentToDelete);
                db.SaveChanges();
            }
            return RedirectToAction("CourseConnections", new { courseID = courseID});
        }

        public ActionResult DeleteCourse(int? id)
        {
            if(id.HasValue)
            {
                int ID = id.Value;
                AdminCourseOverView viewModel = CourseService.GetCourseByID(ID);
                if(viewModel != null)
                {
                    return View(viewModel);
               }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult DeleteCourse(AdminCourseOverView newData)
        {
            
                Course courseToDelete = db.Courses.Where(x => x.ID == newData.ID).SingleOrDefault();
                db.Courses.Remove(courseToDelete);
                db.SaveChanges();
                
            
            return RedirectToAction("Index");
        }



       public ActionResult DeletePerson(string id)
        {
            if (id != null)
            {
                var user = UserManager.FindById(id);
                RegisterViewModel viewModel = new RegisterViewModel
                {
                    ID = user.Id,
                    FullName = user.FullName,
                    UserName = user.UserName,
                    SSN = user.Ssn,
                    Email = user.Email
                };
                return View(viewModel);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeletePerson(RegisterViewModel user)
        {
            if (user.ID != null)
            {
                var deluser = await UserManager.FindByIdAsync(user.ID);
                var result = await UserManager.DeleteAsync(deluser);
                if (result.Succeeded)
                {
                    db.SaveChanges();
                    await db.SaveChangesAsync();
                }
                return RedirectToAction("Index", "Admin");
            }
        // If we got this far, something failed, redisplay form
            return RedirectToAction("Index", "fdsfsda");
        }
        

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

    }
}