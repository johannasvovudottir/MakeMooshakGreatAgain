using Microsoft.AspNet.Identity;
using RipCore.Models;
using RipCore.Models.Entities;
using RipCore.Models.ViewModels;
using RipCore.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace RipCore.Controllers
{
    public class UserController : BaseController
    {
        private CourseService service = new CourseService();
        private AccountsService accountService = new AccountsService();
        private AssignmentsService assignmentService = new AssignmentsService();
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            #region Security
            SecurityRedirect redirect = accountService.VerifySecurityLevel
                (
                    auth:       User.Identity.IsAuthenticated,
                    secLevel:   SecurityState.USER,
                    userID:     User.Identity.GetUserId(),
                    courseID:   null
                );
            if(redirect.Redirect)
            {
                return RedirectToAction(redirect.ActionName, redirect.ControllerName);
            }
            #endregion

            var viewModels = service.GetAllInfo(User.Identity.GetUserId());
            viewModels.Name = User.Identity.Name;
            return View(viewModels);
        }

        public ActionResult StudentOverview(int id)
        {
            #region Security
            SecurityRedirect redirect = accountService.VerifySecurityLevel
                (
                    auth: User.Identity.IsAuthenticated,
                    secLevel: SecurityState.STUDENT,
                    userID: User.Identity.GetUserId(),
                    courseID: id
                );
            if (redirect.Redirect)
            {
                return RedirectToAction(redirect.ActionName, redirect.ControllerName);
            }
            #endregion

            CourseViewModel viewModel = service.GetCoursesById(id, User.Identity.GetUserId());
            viewModel.isTeacher = false;
            return View(viewModel);
        }

        public ActionResult TeacherOverview(int id)
        {
            #region Security
            SecurityRedirect redirect = accountService.VerifySecurityLevel
                (
                    auth: User.Identity.IsAuthenticated,
                    secLevel: SecurityState.TEACHER,
                    userID: User.Identity.GetUserId(),
                    courseID: id
                );
            if (redirect.Redirect)
            {
                return RedirectToAction(redirect.ActionName, redirect.ControllerName);
            }
            #endregion

            CourseViewModel viewModel = service.GetCoursesById(id, User.Identity.GetUserId());
            viewModel.isTeacher = true;
            return View(viewModel);
        }

        public ActionResult Delete(int id)
        {
            Assignment assignment = (from a in db.Assignments where a.ID == id select a).FirstOrDefault();
            int courseID = assignment.CourseID;
            #region Security
            SecurityRedirect redirect = accountService.VerifySecurityLevel
                (
                    auth: User.Identity.IsAuthenticated,
                    secLevel: SecurityState.TEACHER,
                    userID: User.Identity.GetUserId(),
                    courseID: courseID
                );
            if (redirect.Redirect)
            {
                return RedirectToAction(redirect.ActionName, redirect.ControllerName);
            }
            #endregion
            assignmentService.deleteAssignment(assignment);
            //if (assignment != null)
            //{
            //    List<Milestone> milestones = (from m in db.Milestones where m.AssignmentID == id select m).ToList();
            //    if(milestones.Count != 0)
            //    {
            //        IEnumerable<Milestone> milestonesToDelete = milestones;
            //        db.Milestones.RemoveRange(milestonesToDelete);
            //        db.SaveChanges();
            //    }
            //    db.Assignments.Remove(assignment);
            //    db.SaveChanges();
            //}
            return RedirectToAction("TeacherOverview", new { id = courseID });
         }

        public ActionResult DeleteMilestone(int id)
        {
            Milestone milestone = (from m in db.Milestones where m.ID == id select m).FirstOrDefault();
            int assignmentID = milestone.AssignmentID;
            int courseID = (from a in db.Assignments where a.ID == assignmentID select a.CourseID).FirstOrDefault();
            #region Security
            SecurityRedirect redirect = accountService.VerifySecurityLevel
                (
                    auth: User.Identity.IsAuthenticated,
                    secLevel: SecurityState.TEACHER,
                    userID: User.Identity.GetUserId(),
                    courseID: courseID
                );
            if (redirect.Redirect)
            {
                return RedirectToAction(redirect.ActionName, redirect.ControllerName);
            }
            #endregion
            assignmentService.deleteMilestone(milestone);
            //if (milestone != null)
            //{
            //    List<Solution> solutions = (from s in db.Solutions where s.MilestoneID == id select s).ToList();
            //    List<Submission> submissions = (from s in db.Submission where s.MilestoneID == id select s).ToList();
            //    if (solutions.Count != 0)
            //    {
            //        IEnumerable<Solution> solutionsToDelete = solutions;
            //        db.Solutions.RemoveRange(solutionsToDelete);
            //        db.SaveChanges();
            //    }

            //    if (submissions.Count != 0)
            //    {
            //        IEnumerable<Submission> submissionsToDelete = submissions;
            //        db.Submission.RemoveRange(submissionsToDelete);
            //        db.SaveChanges();
            //    }
            //    db.Milestones.Remove(milestone);
            //    db.SaveChanges();
            //}
            return RedirectToAction("TeacherAssignmentView", new { id = assignmentID });
        }
        

        public ActionResult Create(int id)
        {
            #region Security
            if(!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            #endregion

            AssignmentViewModel viewModel = new AssignmentViewModel();
            viewModel.CourseID = id;
            viewModel.Milestones = new List<AssignmentMilestoneViewModel>();
            AssignmentMilestoneViewModel milestone = new AssignmentMilestoneViewModel { Title = "", AssignmentID = id };
            viewModel.Milestones.Add(milestone);
            viewModel.programmingLanguages = assignmentService.GetProgrammingLanguages();
            viewModel.DueDate = DateTime.Now;
            viewModel.DateCreated = DateTime.Now;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(AssignmentViewModel newData, int counter, FormCollection collection, IEnumerable<HttpPostedFileBase> files)
        {
            #region Security
            SecurityRedirect redirect = accountService.VerifySecurityLevel
                (
                    auth: User.Identity.IsAuthenticated,
                    secLevel: SecurityState.TEACHER,
                    userID: User.Identity.GetUserId(),
                    courseID: newData.CourseID
                );
            if (redirect.Redirect)
            {
                return RedirectToAction(redirect.ActionName, redirect.ControllerName);
            }
            #endregion
            if (ModelState.IsValid)
            {
                int tmp = newData.CourseID;
                Assignment assignemnt = new Assignment { CourseID = newData.CourseID, DateCreated = newData.DateCreated, Description = newData.Description, DueDate = newData.DueDate, TestCases = newData.TestCases, ProgrammingLanguageID = newData.ProgrammingLanguageID, Title = newData.Title, Weight = newData.Weight };
                db.Assignments.Add(assignemnt);
                db.SaveChanges();
                int assignmentID = (from a in db.Assignments where a.Title == newData.Title && a.CourseID == newData.CourseID select a.ID).FirstOrDefault();
                //if (newData.File != null || newData.Milestones.Count <= 1)
                //{
                    string milestoneZeroTestCases = collection["Milestones[" + 0 + "].TestCases"];
                    if (assignmentID != 0)
                    {
                        Milestone milestone = new Milestone
                        {
                            Title = newData.Title,
                            Weight = newData.Weight,
                            Description = newData.Description,
                            TestCases = milestoneZeroTestCases,
                            AssignmentID = assignmentID,
                            DateCreated = newData.DateCreated,
                            DueDate = newData.DueDate,
                            ProgrammingLanguageID = newData.ProgrammingLanguageID
                        };
                        db.Milestones.Add(milestone);
                        db.SaveChanges();
                    }
                //}
                string bla = collection["Milestones[" + 0 + "].ID"];
                for (int i = 1; i < counter; i++)
                {
                    //bool exists = collection["Milestones[" + i + "].ID"] != "0";
                    //if (!exists)
                    //{
                    string title = collection["Milestones[" + i + "].Title"];
                    int weight;
                    Int32.TryParse(collection["Milestones[" + i + "].Weight"], out weight);
                    string description = collection["Milestones[" + i + "].Description"];
                    string testCases = collection["Milestones[" + i + "].TestCases"];
                    if (assignmentID != 0)
                    {
                        Milestone milestone = new Milestone
                        {
                            Title = title,
                            Weight = weight,
                            Description = description,
                            TestCases = testCases,
                            AssignmentID = assignmentID,
                            DateCreated = newData.DateCreated,
                            DueDate = newData.DueDate,
                            ProgrammingLanguageID = newData.ProgrammingLanguageID
                        };
                        db.Milestones.Add(milestone);
                        db.SaveChanges();
                    }

                    //}
                }
                return RedirectToAction("TeacherOverview", new { id = newData.CourseID });
            }
            newData.programmingLanguages = assignmentService.GetProgrammingLanguages();
            newData.Milestones = new List<AssignmentMilestoneViewModel>();
            return View(newData);
        }

        public ActionResult Edit(int id)
        {
            #region Security
            string ID = null;
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            if (!accountService.GetIdByUser(User.Identity.Name, ref ID))
                return RedirectToAction("Index", "Home");

            if (id <= 0)
            {
                return View();
            }
            #endregion
            //if(id.HasValue)
            //{ 
            AssignmentViewModel viewModel = assignmentService.GetAssignmentsById(id);
            if (viewModel != null)
            {
                return View(viewModel);
            }
            //}
            return RedirectToAction("TeacherOverview", new { id = id });
        }

        /*   [HttpPost]
           public ActionResult Edit(AssignmentViewModel model, int counter, FormCollection collection)
           {
               #region Security
               SecurityRedirect redirect = accountService.VerifySecurityLevel
                   (
                       auth: User.Identity.IsAuthenticated,
                       secLevel: SecurityState.TEACHER,
                       userID: User.Identity.GetUserId(),
                       courseID: model.CourseID
                   );
               if (redirect.Redirect)
               {
                   return RedirectToAction(redirect.ActionName, redirect.ControllerName);
               }
               #endregion

               if (ModelState.IsValid)
               {
                   for (int i = 0; i < counter; i++)
                   {
                       bool exists = collection["Milestones[" + i + "].ID"] != null;
                       if (!exists)
                       {
                           string title = collection["Milestones[" + i + "].Title"];
                           int weight;
                           Int32.TryParse(collection["Milestones[" + i + "].Weight"], out weight);
                           string description = collection["Milestones[" + i + "].Description"];

                           db.Milestones.Add(new Milestone()
                           {
                               Title = title,
                               Weight = weight,
                               Description = description,
                               AssignmentID = model.ID
                           });
                       }
                   }

                   Assignment assignment = db.Assignments.Where(x => x.ID == model.ID).SingleOrDefault();
                   if (assignment != null)
                   {
                       assignment.Title = model.Title;
                       assignment.Description = model.Description;
                       assignment.DateCreated = model.DateCreated;
                       assignment.DueDate = model.DueDate;
                       assignment.ProgrammingLanguageID = model.ProgrammingLanguageID;
                       if (model.File != null)
                       {
                           using (MemoryStream memoryStream = new MemoryStream())
                           {
                               model.File.InputStream.CopyTo(memoryStream);
                               assignment.TestCases = Encoding.ASCII.GetString(memoryStream.ToArray());
                           }
                       }
                       db.SaveChanges();
                   }
                   return RedirectToAction("Index");

               }
               return View(model);
           }*/

        [HttpPost]
        public ActionResult Edit(AssignmentViewModel model, int counter, FormCollection collection)
        {
            #region Security
            SecurityRedirect redirect = accountService.VerifySecurityLevel
                (
                    auth: User.Identity.IsAuthenticated,
                    secLevel: SecurityState.TEACHER,
                    userID: User.Identity.GetUserId(),
                    courseID: model.CourseID
                );
            if (redirect.Redirect)
            {
                return RedirectToAction(redirect.ActionName, redirect.ControllerName);
            }
            #endregion

            if (ModelState.IsValid)
            {
                for (int i = 0; i < counter; i++)
                {
                    bool exists = collection["Milestones[" + i + "].ID"] != null;
                    if (!exists)
                    {
                        string title = collection["Milestones[" + i + "].Title"];
                        int weight;
                        Int32.TryParse(collection["Milestones[" + i + "].Weight"], out weight);
                        string description = collection["Milestones[" + i + "].Description"];
                        string testCases = collection["Milestones[" + i + "].TestCases"];
                        db.Milestones.Add(new Milestone()
                        {
                            Title = title,
                            Weight = weight,
                            Description = description,
                            AssignmentID = model.ID,
                            TestCases = testCases,
                            DateCreated = model.DateCreated,
                            DueDate = model.DueDate,
                            ProgrammingLanguageID = model.ProgrammingLanguageID
                        });
                    }
                }

                Assignment assignment = db.Assignments.Where(x => x.ID == model.ID).SingleOrDefault();
                if (assignment != null)
                {
                    assignment.Title = model.Title;
                    assignment.Description = model.Description;
                    assignment.DateCreated = model.DateCreated;
                    assignment.DueDate = model.DueDate;
                    assignment.ProgrammingLanguageID = model.ProgrammingLanguageID;
                    if (model.File != null)
                    {
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            model.File.InputStream.CopyTo(memoryStream);
                            assignment.TestCases = Encoding.ASCII.GetString(memoryStream.ToArray());
                        }
                    }
                    db.SaveChanges();
                }
                return RedirectToAction("Index");

            }
            return View(model);
        }

        public ActionResult StudentAssignmentView(int id)
        {
            #region Security
            string userID = null;
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (!accountService.GetIdByUser(User.Identity.Name, ref userID))
                return RedirectToAction("Index", "Home");

            //if (!accountService.IsUserQualified("Teacher", userID, id) || !accountService.IsUserQualified("Student", userID, id))
              //  return RedirectToAction("Index", "User");


            if (id <= 0)
                return View();
            #endregion
            AssignmentViewModel viewModel = assignmentService.GetAssignmentForView(id, false);
            //if(limitSubmisssions != null)
            //{
            //    List<int> milestonesID = (from m in db.Milestones where m.AssignmentID == id select m.ID).ToList();
            //    foreach (var item in milestonesID)
            //    {
            //        int numberOfSubmissions = (from m in db.Milestones where m.ID == milestoneID && m.userID == uerid select m).ToList().Count;
            //        if(numberOfSubmissions >= limit)
            //        {
            //            SelectListItem item = new SelectListItem { Value = milestonesID, Text = Milestone name}
            //            viewModel.milestoneNumber.Remove(item);
            //        }
            //    }
            return View(viewModel);
        }

        public ActionResult TeacherAssignmentView(int id)
        {
            #region Security
            string userID = null;
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (!accountService.GetIdByUser(User.Identity.Name, ref userID))
                return RedirectToAction("Index", "Home");

            //if (!accountService.IsUserQualified("Teacher", userID, id))
              //  return RedirectToAction("Index", "User");

            if (id <= 0)
                return View();
            #endregion

            AssignmentViewModel viewModel = assignmentService.GetAssignmentForView(id, true);
            viewModel.UserID = User.Identity.GetUserId();
            return View(viewModel);
        }

        public ActionResult AddMilestone(FormCollection collection, HttpPostedFileBase files)
        {
            var test = collection["files"];
            var test2 = Request.Files["files"];
            var test3 = files;
            string TestCases;
            //if (String.IsNullOrEmpty(collection["addMilestoneTitle"]))
            //{
            //    ModelState.AddModelError("name", "Name is required");
            //    return Json("bla", JsonRequestBehavior.AllowGet);
            //    //return RedirectToAction("IndexJSON", "MovieApp", new { id = movieId });
            //}

            using (MemoryStream memoryStream = new MemoryStream())
            {
                files.InputStream.CopyTo(memoryStream);
                TestCases = Encoding.ASCII.GetString(memoryStream.ToArray());
            }

            //if (String.IsNullOrEmpty(TestCases))
            //{
            //    return Json("", JsonRequestBehavior.AllowGet);
            //    //return RedirectToAction("IndexJSON", "MovieApp", new { id = movieId });
            //}

            //if (String.IsNullOrEmpty(collection["addMilestoneTitle"]))
            //{
            //    ModelState.AddModelError("name", "Name is required");
            //    return Json("bla", JsonRequestBehavior.AllowGet);
            //    //return RedirectToAction("IndexJSON", "MovieApp", new { id = movieId });
            //}

            //if (String.IsNullOrEmpty(TestCases))
            //{
            //    return Json("", JsonRequestBehavior.AllowGet);
            //    //return RedirectToAction("IndexJSON", "MovieApp", new { id = movieId });
            //}
            return Json(TestCases, JsonRequestBehavior.AllowGet);
        }

    }
}