using RipCore.Models;
using RipCore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RipCore.Services
{
    public class UsersService
    {
        private ApplicationDbContext db;
        public UsersService()
        {
            db = new ApplicationDbContext();
        }
        public List<CourseViewModel> GetAssignmentsInCourse(int courseId)
        {
            return null;
        }

        public CourseViewModel GetCoursesById(int courseID)
        {
            var course = db.Courses.SingleOrDefault(x => x.ID == courseID);
            if (course == null)
            {
                //TODO kasta
            }
            /*
            var milestones = db.Milestones.Where(x => x.AssignmendId == assignmentID).Select(x => new AssignmentMilestoneViewModel
            {
                Title = x.Title
            }).ToList();
            */
            var viewModel = new CourseViewModel
            {
                Name = course.Name
            };
            return viewModel;
        }
    }
}