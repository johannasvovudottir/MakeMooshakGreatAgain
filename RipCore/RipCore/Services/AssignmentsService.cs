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
            List <AssignmentViewModel> assignentViewModel = new List<AssignmentViewModel>();
            foreach (var item in assignments)
            {
                AssignmentViewModel tmp = new AssignmentViewModel
                {
                    Title = item.Title,
                    Weight = item.Weight
                };
                assignentViewModel.Add(tmp);
            }
            return assignentViewModel;
        }

        public AssignmentViewModel GetAssignmentsById(int assignmentID)
        {
            var assignment = db.Assignments.SingleOrDefault(x => x.ID == assignmentID);
            if (assignment == null)
            {
                //TODO kasta
            }
            /*
            var milestones = db.Milestones.Where(x => x.AssignmendId == assignmentID).Select(x => new AssignmentMilestoneViewModel
            {
                Title = x.Title
            }).ToList();
            */
            var viewModel = new AssignmentViewModel
            {
                Title = assignment.Title
            };
            return null;
        }
    }
}