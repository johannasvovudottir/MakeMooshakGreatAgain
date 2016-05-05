using RipCore.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RipCore.Models.ViewModels
{
    public class CourseViewModel
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public int Year { get; set; }
        public string Semester { get; set; }
        public string UserName { get; set; }
        public string UserID { get; set; }
        public List<AssignmentViewModel> Assignments { get; set; }
        public List<ApplicationUser> Teachers { get; set; }
        public List<ApplicationUser> Students { get; set; }
        public List<Course> CoursesAsStudent { get; set; }
        public List<Course> CoursesAsTeacher { get; set; }
        public bool isTeacher { get; set; }
    }
}