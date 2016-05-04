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
    public class UserController : Controller
    {
        private CourseService service = new CourseService();
        private AccountsService accountService = new AccountsService();
        private AssignmentsService assignmentService = new AssignmentsService();
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Student
        public ActionResult Index()
        {
            #region Security
            int ID = 16;
            //if (!User.Identity.IsAuthenticated)
              //  return RedirectToAction("Login", "Account");

            //if (!accountService.GetIdByUser(User.Identity.Name, ref ID))
              //  return RedirectToAction("Index", "Home");
            #endregion

            var viewModels = service.GetAllInfo(ID);
            viewModels.Name = User.Identity.Name;
            return View(viewModels);
        }

        public ActionResult StudentOverview(int id, int userID)
        {
            #region Security
            //int actualID = 0;
            //if (!User.Identity.IsAuthenticated)
              //  return RedirectToAction("Login", "Account");

           /* if (!accountService.GetIdByUser(User.Identity.Name, ref actualID))
                return RedirectToAction("Index", "Home");

            if(!accountService.IsUserQualified("Teacher", actualID, id));
            {
                if (!accountService.IsUserQualified("Student", actualID, id))
                {
                    return RedirectToAction("Index", "User");
                }
            }
            if (userID != actualID)
            {
                var model = service.GetCoursesById(id, actualID);
                model.UserID = actualID;
                model.isTeacher = false;
                return View(model);
            }*/
            #endregion

            var viewModel = service.GetCoursesById(id, userID);
            viewModel.isTeacher = false;
            return View(viewModel);
        }

        public ActionResult TeacherOverview(int id, int userID)
        {
            #region Security
           // int actualID = 0;
            /*if(!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            if (!accountService.GetIdByUser(User.Identity.Name, ref actualID))
                return RedirectToAction("Index", "Home");

            if (!accountService.IsUserQualified("Teacher", actualID, id)) 
            {
                return RedirectToAction("Index", "User");
            }

            if (userID != actualID)
            {
                var model = service.GetCoursesById(id, actualID);
                model.UserID = actualID;
                model.isTeacher = true;
                return View(model);
            }*/
            #endregion

            var viewModel = service.GetCoursesById(id, userID);
            viewModel.isTeacher = true;
            return View(viewModel);
        }

        public ActionResult Create(int id)
        {
            #region Security
            int ID = 0;
           // if (!User.Identity.IsAuthenticated)
            //    return RedirectToAction("Index", "Home");

//            if (!accountService.GetIdByUser(User.Identity.Name, ref ID))
  //              return RedirectToAction("Index", "Home");
            #endregion

            AssignmentViewModel viewModel = new AssignmentViewModel();
            viewModel.CourseID = id;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(AssignmentViewModel newData)
        {
            #region Security
            int ID = 16;
    /*        if(User.Identity.IsAuthenticated)
            {
                accountService.GetIdByUser(User.Identity.Name, ref ID);
            }
            if(!accountService.IsUserQualified("Teacher", ID, newData.CourseID))
            {
                return RedirectToAction("Index", "User");
            }
      */      
      #endregion

            int tmp = newData.CourseID;
            Assignment newAssignment = new Assignment { Title = newData.Title, CourseID = newData.CourseID, Weight = newData.Weight, DueDate = newData.DueDate, DateCreated = newData.DateCreated, Description = newData.Description };
            UpdateModel(newAssignment);
            db.Assignments.Add(newAssignment);
            db.SaveChanges();
            return RedirectToAction("TeacherOverview", new { id=newData.CourseID, userID = ID});
        }

        public ActionResult Edit(int id)
        {
            #region Security
            int ID = 16;
           /* if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            if (!accountService.GetIdByUser(User.Identity.Name, ref ID))
                return RedirectToAction("Index", "Home");

            if(id <= 0)
            {
                return View();
            }*/
            #endregion
            //if(id.HasValue)
            //{
            AssignmentViewModel viewModel = assignmentService.GetAssignmentsById(id);
                if(viewModel != null)
                {
                    return View(viewModel);
                }
            //}
            return RedirectToAction("TeacherOverview", new { id = id, userID = ID });
        }

        [HttpPost]
        public ActionResult Edit(AssignmentViewModel model, int counter, FormCollection collection)
        {
            #region Security
            int ID = 0;
            /*if (User.Identity.IsAuthenticated)
            {
                accountService.GetIdByUser(User.Identity.Name, ref ID);
            }
            if (!accountService.IsUserQualified("Teacher", ID, model.CourseID))
            {
                //return RedirectToAction("Index", "User");
            }*/
            #endregion

            if (ModelState.IsValid)
            {
                for(int i = 0; i < counter; i++)
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
                    db.SaveChanges();
                }
                return RedirectToAction("Index");

            }
            return View(model);
        }
        
        public ActionResult StudentAssignmentView(int id, int userID)
        {
            AssignmentViewModel viewModel = assignmentService.GetAssignmentForView(id, false);
            return View(viewModel);
        }

        public ActionResult TeacherAssignmentView(int id, int userID)
        {
            AssignmentViewModel viewModel = assignmentService.GetAssignmentForView(id, true);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult SubmitSolution(AssignmentViewModel viewModel, int id, int userID)
        {
            Solution solution = new Solution { AssignmentID = id, StudentID = userID, SolutionOutput = viewModel.Solution };
            //TODO TJEKKA STATUS  A LAUSN
            return RedirectToAction("StudentOverview", new { id = viewModel.CourseID, userID = 16 } );
        }

        public ActionResult AddMilestone(int id)
        {
            AssignmentViewModel viewModel = assignmentService.GetAssignmentForView(id, true);
            return View(viewModel);
        }
    }
}