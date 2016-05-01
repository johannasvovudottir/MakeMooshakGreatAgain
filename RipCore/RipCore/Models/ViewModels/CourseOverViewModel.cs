using RipCore.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RipCore.Models.ViewModels
{
    public class CourseOverViewModel
    {
        public List<Course> whereTeacher { get; set; }
        public List<Course> whereStudent { get; set; }
        public List<AssignmentViewModel> assignments { get; set; }
    }
}