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
        private CourseService CourseService = new CourseService();


        public AssignmentsService(IAppDataContext dataContext = null)
        {
            db = dataContext ?? new ApplicationDbContext();

        }

        /*
        private ApplicationDbContext db;
        private CourseService CourseService = new CourseService();
        public AssignmentsService()
        {
            db = new ApplicationDbContext();
        }*/

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

        public AssignmentViewModel SetModel(Assignment assignment, List<AssignmentMilestoneViewModel> milestoneViewModel)
        {
            AssignmentViewModel viewModel = new AssignmentViewModel
            {
                Title = assignment.Title,
                Weight = assignment.Weight,
                Description = assignment.Description,
                ID = assignment.ID,
                CourseID = assignment.CourseID,
                CourseName = CourseService.getCourseNameByID(assignment.CourseID),
                ProgrammingLanguageID = assignment.ProgrammingLanguageID,
                NumberOfHandins = getNumberOfHandIns(assignment.ID),
                NumberOfNotHandedIn = CourseService.GetAllStudents(assignment.CourseID).Count- getNumberOfHandIns(assignment.ID),
                DateCreated = assignment.DateCreated,
                Milestones = milestoneViewModel,
                DueDate = assignment.DueDate,
                TestCases = assignment.TestCases,
                programmingLanguages = GetProgrammingLanguages()   
            };
            return viewModel;
        }
        public AssignmentViewModel GetAssignmentForView(int assignmentID, bool teacher)
        {
            AssignmentViewModel viewModel = GetAssignmentsById(assignmentID);
            viewModel.IsTeacher = teacher;
            viewModel.milestoneNumber = GetMilestonesNumber(assignmentID);
            return viewModel;
        }

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

        public CourseViewModel GetGrades(string userID, CourseViewModel modelToAddTo)
        {
            foreach (var item in modelToAddTo.Assignments) {
                item.Grade = GetGradeByAssignment(userID,item.ID);
            }
            return modelToAddTo;
        }

        public List<Solution> GetSolutionsById(string userID, int assignmentID)
        {
            var result = (from c in db.Milestones
                          join cn in db.Solutions on userID equals cn.StudentID
                          where (assignmentID == c.AssignmentID)
                          select cn).ToList();
            return result;
        }

        public double GetGradeByAssignment(string userID, int assignmentID)
        {
            List<Solution> userSolutions = GetSolutionsById(userID, assignmentID);
            double totalGrade = 0;
            foreach (var item in userSolutions)
            {
                Milestone currentMilestone = GetMilestoneBySolution(item);
                totalGrade += (Convert.ToDouble(item.Grade) * currentMilestone.Weight * 0.01);
            }
            return totalGrade;
        }

        public Milestone GetMilestoneBySolution(Solution userSolution)
        {
            var result = (from c in db.Milestones
                          where c.ID == userSolution.MilestoneID
                          select c).SingleOrDefault();
            return result; 
        }
        public string GetProgrammingLanguageByID(int languageID)
        {
            string[] languages = { ".cpp", ".cs", ".c", "regex", "other", "otherNotTests" };
            if (languageID > 5 || languageID <= 0)
                languageID = 1;
            return languages[languageID-1];
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
                    //db.Solutions.RemoveRange(solutionsToDelete);
                    db.SaveChanges();
                }

                if (submissions.Count != 0)
                {
                    IEnumerable<Submission> submissionsToDelete = submissions;
                    foreach (var item in submissionsToDelete)
                    {
                        db.Submission.Remove(item);
                    }
                    //db.Submission.RemoveRange(submissionsToDelete);
                    db.SaveChanges();
                }
                Milestone milestoneToDelete = db.Milestones.Where(x => x.ID == milestone.ID).SingleOrDefault();
                db.Milestones.Remove(milestoneToDelete);
                db.SaveChanges();

               // db.Milestones.Remove(milestone);
               // db.SaveChanges();
            }
        }

        public void deleteAssignment(Assignment assignment)
        {
            if (assignment != null)
            {
                List<Milestone> milestones = (from m in db.Milestones where m.AssignmentID == assignment.ID select m).ToList();
                foreach(var item in milestones)
                {
                    deleteMilestone(item);
                }
                /*db.Assignments.Remove(assignment);
                db.SaveChanges();*/

                Assignment assignmentToDelete = db.Assignments.Where(x => x.ID == assignment.ID).SingleOrDefault();
                db.Assignments.Remove(assignmentToDelete);
                db.SaveChanges();
            }
        }

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