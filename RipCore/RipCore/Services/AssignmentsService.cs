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
                AssignmentViewModel tmp = new AssignmentViewModel
                {
                    Title = item.Title,
                    Weight = item.Weight,
                    ID = item.ID,
                    IsTeacher = false,
                    Description = item.Description,
                    CourseID = item.CourseID,
                    DateCreated = item.DateCreated,
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
            /*
            var milestones = db.Milestones.Where(x => x.AssignmendId == assignmentID).Select(x => new AssignmentMilestoneViewModel
            {
                Title = x.Title
            }).ToList();
            */
            var viewModel = new AssignmentViewModel
            {
                Title = item.Title,
                Weight = item.Weight,
                Description = item.Description,
                ID = item.ID,
                IsTeacher = false,
                CourseID = item.CourseID,
                DateCreated = item.DateCreated,
                DueDate = item.DueDate
            };
            return viewModel;
        }

        public AssignmentViewModel GetAssignmentForView(int assignmentID, bool teacher)
        {
            AssignmentViewModel viewModel = GetAssignmentsById(assignmentID);
            viewModel.IsTeacher = teacher;
            return viewModel;
        }
    }
}