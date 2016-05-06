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
        private ApplicationDbContext db;
        public AssignmentsService()
        {
            db = new ApplicationDbContext();
        }

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
                ProgrammingLanguageID = assignment.ProgrammingLanguageID,
                DateCreated = assignment.DateCreated,
                Milestones = milestoneViewModel,
                DueDate = assignment.DueDate,
                Input = assignment.Input,
                Output = assignment.Output,
                programmingLanguages = GetProgrammingLanguages()   
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

        public List<SelectListItem> GetProgrammingLanguages()
        {
            List<SelectListItem> programmingLanguages = new List<SelectListItem>();

            programmingLanguages.Add(new SelectListItem() { Value = "1", Text = "C++" });
            programmingLanguages.Add(new SelectListItem() { Value = "2", Text = "C#" });
            programmingLanguages.Add(new SelectListItem() { Value = "3", Text = "C" });
            programmingLanguages.Add(new SelectListItem() { Value = "4", Text = "Python" });
            programmingLanguages.Add(new SelectListItem() { Value = "5", Text = "Java" });
            return programmingLanguages;
        }
    }
}