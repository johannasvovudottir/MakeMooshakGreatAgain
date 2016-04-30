using RipCore.Models;
using RipCore.Models.ViewModels;
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
            return null;
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