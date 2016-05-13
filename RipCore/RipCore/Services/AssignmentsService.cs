using RipCore.Models;
using RipCore.Models.ViewModels;
using RipCore.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RipCore.Services
{
    public class AssignmentsService
    {
        private readonly IAppDataContext db;


        public AssignmentsService(IAppDataContext dataContext = null)
        {
            db = dataContext ?? new ApplicationDbContext();

        }

        /// <summary>
        /// A function that returns a list of all assignments
        /// in a given course
        /// </summary>
        public List<AssignmentViewModel> GetAssignmentsInCourse(int courseId)
        {
            List<Assignment> assignments = (from a in db.Assignments
                                            where a.CourseID == courseId
                                            select a).ToList();
            List<AssignmentViewModel> assignentViewModel = new List<AssignmentViewModel>();

            foreach (var item in assignments)
            {
                List<AssignmentMilestoneViewModel> milestones = GetMilestones(item.ID);
                AssignmentViewModel tmp = SetModel(item, milestones);
                assignentViewModel.Add(tmp);
            }
            return assignentViewModel;
        }
        /// <summary>
        /// A function that returns a list of assignmentviewmodels
        /// given a specific user ID
        /// </summary>
        public List<AssignmentViewModel> GetAllUserAssignments(string userID)
        {
            CourseService temp = new CourseService();
            var allStudentCourses = temp.GetCoursesWhereStudent(userID);
            var allTeacherCourses = temp.GetCoursesWhereTeacher(userID);
            List<AssignmentViewModel> allAssignments = new List<AssignmentViewModel>();
            AssignmentsService tmp = new AssignmentsService();
            foreach (var item in allStudentCourses)
            {
                allAssignments.AddRange(tmp.GetAssignmentsInCourse(item.ID));
            }
            foreach (var item in allTeacherCourses)
            {
                var teacherAssignments = tmp.GetAssignmentsInCourse(item.ID);
                foreach (var entry in teacherAssignments)
                {
                    entry.IsTeacher = true;
                }
                allAssignments.AddRange(teacherAssignments);
            }
            return allAssignments;
        }
        /// <summary>
        /// A function that returns a specific assignment
        /// given the assignmentID
        /// </summary>
        public AssignmentViewModel GetAssignmentsById(int assignmentID)
        {
            Assignment item = (from a in db.Assignments
                               where a.ID == assignmentID
                               select a).SingleOrDefault();
            if (item == null)
            {
                //TODO kasta
                return null;
            }

            List<AssignmentMilestoneViewModel> milestoneViewModel = GetMilestones(assignmentID);
            AssignmentViewModel viewModel = SetModel(item, milestoneViewModel);
            return viewModel;
        }
        /// <summary>
        /// A function that populates the variables for an
        /// assignmentviewmodel
        /// </summary>
        public AssignmentViewModel SetModel(Assignment assignment, List<AssignmentMilestoneViewModel> milestoneViewModel)
        {
            CourseService tmp = new CourseService();
            AssignmentViewModel viewModel = new AssignmentViewModel
            {
                Title = assignment.Title,
                Weight = assignment.Weight,
                Description = assignment.Description,
                ID = assignment.ID,
                CourseID = assignment.CourseID,
                CourseName = tmp.getCourseNameByID(assignment.CourseID),
                ProgrammingLanguageID = assignment.ProgrammingLanguageID,
                NumberOfHandins = getNumberOfHandIns(assignment.ID),
                NumberOfNotHandedIn = tmp.GetAllStudents(assignment.CourseID).Count - getNumberOfHandIns(assignment.ID),
                DateCreated = assignment.DateCreated,
                Milestones = milestoneViewModel,
                DueDate = assignment.DueDate,
                TestCases = assignment.TestCases,
                programmingLanguages = GetProgrammingLanguages()
            };
            return viewModel;
        }
        /// <summary>
        /// A function that gets an assignmentviewmodel for a specific assignment
        /// </summary>
        public AssignmentViewModel GetAssignmentForView(int assignmentID, bool teacher)
        {
            AssignmentViewModel viewModel = GetAssignmentsById(assignmentID);
            viewModel.IsTeacher = teacher;
            viewModel.milestoneNumber = GetMilestonesNumber(assignmentID);
            viewModel.ProgrammingLanguage = GetProgrammingLanguageByID(viewModel.ProgrammingLanguageID);
            return viewModel;
        }
        /// <summary>
        /// A function that returns a list of all the milestones
        /// for a specific 
        /// </summary>
        public List<AssignmentMilestoneViewModel> GetMilestones(int assignmentID)
        {
            var milestones = (from m in db.Milestones
                              where m.AssignmentID == assignmentID
                              select m).ToList();
            List<AssignmentMilestoneViewModel> milestoneViewModel = new List<AssignmentMilestoneViewModel>();
            foreach (var item in milestones)
            {
                AssignmentMilestoneViewModel tmp = new AssignmentMilestoneViewModel
                {
                    Title = item.Title,
                    Weight = item.Weight,
                    Description = item.Description,
                    AssignmentID = item.AssignmentID,
                    ID = item.ID
                };
                milestoneViewModel.Add(tmp);
            }
            return milestoneViewModel;
        }
        /// <summary>
        /// A function that returns a selectlistitem list of all 
        /// the availible programming languages in the system
        /// </summary>
        public List<SelectListItem> GetProgrammingLanguages()
        {
            List<SelectListItem> programmingLanguages = new List<SelectListItem>();

            programmingLanguages.Add(new SelectListItem() { Value = "1", Text = "C++" });
            programmingLanguages.Add(new SelectListItem() { Value = "2", Text = "C#" });
            programmingLanguages.Add(new SelectListItem() { Value = "3", Text = "C" });
            programmingLanguages.Add(new SelectListItem() { Value = "4", Text = "RegEx" });
            programmingLanguages.Add(new SelectListItem() { Value = "5", Text = "Other (with tests)" });
            programmingLanguages.Add(new SelectListItem() { Value = "5", Text = "Other (without tests)" });
            return programmingLanguages;
        }
        /// <summary>
        /// A function that adds grades to a courseviewmodel
        /// </summary>
        public CourseViewModel GetGrades(string userID, CourseViewModel modelToAddTo)
        {
            foreach (var item in modelToAddTo.Assignments)
            {
                item.Grade = GetGradeByAssignment(userID, item.ID);
            }
            return modelToAddTo;
        }
        /// <summary>
        /// A function that returns a list of solutions given a 
        /// specific user and assignment ID
        /// </summary>
        public List<Solution> GetSolutionsById(string userID, int assignmentID)
        {
            List<int> bar = (from m in db.Milestones where m.AssignmentID == assignmentID select m.ID).ToList();
            List<Solution> userSolutions = new List<Solution>();
            foreach (var item in bar)
            {
                List<Solution> solutionList = (from s in db.Solutions where s.MilestoneID == item && s.StudentID == userID select s).ToList();
                userSolutions.AddRange(solutionList);
            }
            return userSolutions;
        }
        /// <summary>
        /// A function that calculates the total grade for a specific
        /// assignment using its milestones
        /// </summary>
        public double GetGradeByAssignment(string userID, int assignmentID)
        {
            List<Solution> userSolutions = GetSolutionsById(userID, assignmentID);
            double totalGrade = 0;
            foreach (var item in userSolutions)
            {
                Milestone currentMilestone = GetMilestoneBySolution(item);
                totalGrade += (Convert.ToDouble(item.Grade) * currentMilestone.Weight) / 100;
            }
            return totalGrade;
        }
        /// <summary>
        /// A function that returns a milestone for a 
        /// specific solution
        /// </summary>
        public Milestone GetMilestoneBySolution(Solution userSolution)
        {
            var result = (from c in db.Milestones
                          where c.ID == userSolution.MilestoneID
                          select c).SingleOrDefault();
            return result;
        }
        /// <summary>
        /// A function that returns a programming language string 
        /// given the language's ID
        /// </summary>
        public string GetProgrammingLanguageByID(int languageID)
        {
            string[] languages = { ".cpp", ".cs", ".c", "regex", "other", "otherNotTests" };
            if (languageID > 5 || languageID <= 0)
                languageID = 1;
            return languages[languageID - 1];
        }
        /// <summary>
        /// A function that return a selectlistitem of milestones
        /// for a specific assignment
        /// </summary>
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
        /// <summary>
        /// A function that deletes a specific milestone, also deletes it's
        /// solutions and submissions
        /// </summary>
        public void deleteMilestone(Milestone milestone)
        {
            if (milestone != null)
            {
                List<Solution> solutions = (from s in db.Solutions where s.MilestoneID == milestone.ID select s).ToList();
                List<Submission> submissions = (from s in db.Submission where s.MilestoneID == milestone.ID select s).ToList();
                if (solutions.Count != 0)
                {
                    IEnumerable<Solution> solutionsToDelete = solutions;
                    foreach (var item in solutionsToDelete)
                    {
                        db.Solutions.Remove(item);
                    }
                    db.SaveChanges();
                }

                if (submissions.Count != 0)
                {
                    IEnumerable<Submission> submissionsToDelete = submissions;
                    foreach (var item in submissionsToDelete)
                    {
                        db.Submission.Remove(item);
                    }
                    db.SaveChanges();
                }
                Milestone milestoneToDelete = db.Milestones.Where(x => x.ID == milestone.ID).SingleOrDefault();
                db.Milestones.Remove(milestoneToDelete);
                db.SaveChanges();

            }
        }
        /// <summary>
        /// A function that deletes a specific assignment
        /// </summary>
        public void deleteAssignment(Assignment assignment)
        {
            if (assignment != null)
            {
                List<Milestone> milestones = (from m in db.Milestones where m.AssignmentID == assignment.ID select m).ToList();
                foreach (var item in milestones)
                {
                    deleteMilestone(item);
                }
            
                Assignment assignmentToDelete = db.Assignments.Where(x => x.ID == assignment.ID).SingleOrDefault();
                db.Assignments.Remove(assignmentToDelete);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// A function that returns the number of total hand ins
        /// for a specific assignment
        /// </summary>
        public int getNumberOfHandIns(int assignmentID)
        {
            var result = (from c in db.Solutions
                          join cn in db.Milestones on c.MilestoneID equals cn.ID
                          join ct in db.Assignments on cn.AssignmentID equals ct.ID
                          join cp in db.Submission on c.SubmissionID equals cp.ID
                          where (cn.AssignmentID == assignmentID)
                          select cp).Count();
            return result;
        }
    }
}