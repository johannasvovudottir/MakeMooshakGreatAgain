using RipCore.Models;
using RipCore.Models.ViewModels;
using RipCore.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RipCore.Services
{
    public class AssignmentsService
    {
        private ApplicationDbContext db;
        public AssignmentsService()
        {
            db = new ApplicationDbContext();
        }
        public List<AssignmentViewModel> GetAssignmentsInCourse(int courseId)
        {
            var assignments = (from a in db.Assignments
                               where a.CourseID == courseId
                               select a).ToList();
            List<AssignmentViewModel> assignentViewModel = new List<AssignmentViewModel>();

            foreach (var item in assignments)
            {
                List<AssignmentMilestoneViewModel> milestones = GetMilestones(item.ID);
                AssignmentViewModel tmp = new AssignmentViewModel
                {
                    Title = item.Title,
                    Weight = item.Weight,
                    ID = item.ID,
                    IsTeacher = false,
                    Description = item.Description,
                    CourseID = item.CourseID,
                    DateCreated = item.DateCreated,
                    Milestones = milestones,
                    MilestoneCount = milestones.Count,
                    DueDate = item.DueDate
                };
                assignentViewModel.Add(tmp);
            }
            return assignentViewModel;
        }

        public AssignmentViewModel GetAssignmentsById(int assignmentID)
        {
            var item = (from a in db.Assignments
                        where a.ID == assignmentID
                        select a).SingleOrDefault();
            if (item == null)
            {
                //TODO kasta
                return null;
            }

            List<AssignmentMilestoneViewModel> milestoneViewModel = GetMilestones(assignmentID);

            AssignmentViewModel viewModel = new AssignmentViewModel
            {
                Title = item.Title,
                Weight = item.Weight,
                Description = item.Description,
                ID = item.ID,
                IsTeacher = false,
                CourseID = item.CourseID,
                DateCreated = item.DateCreated,
                DueDate = item.DueDate,
                Milestones = milestoneViewModel,
                MilestoneCount = milestoneViewModel.Count,
            };
            return viewModel;
        }

        public AssignmentViewModel GetAssignmentForView(int assignmentID, bool teacher)
        {
            AssignmentViewModel viewModel = GetAssignmentsById(assignmentID);
            viewModel.IsTeacher = teacher;
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
                    AssignmentID = item.AssignmentID
                };
                milestoneViewModel.Add(tmp);
            }
            return milestoneViewModel;
        }

        public List<Submission> GetUserSubmissions(int solutionID)
        {
            var submissions = (from s in db.Submission
                               where s.SolutionID == solutionID
                               select s).ToList();
            return null;
        }

        public List<Solution> GetAssignmentSubmissions(int assignmentID)
        {
            var solutions = (from s in db.Solutions
                             where s.AssignmentID == assignmentID
                             select s).ToList();
            List<SolutionViewModel> solutionViewModel = new List<SolutionViewModel>();
            {
                foreach (var item in solutions)
                {

                }
            }
            List<Submission> submissions = new List<Submission>();
            foreach (var item in solutions)
                return null;

            return null;
        }
    }
}