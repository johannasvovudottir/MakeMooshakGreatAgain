using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RipCore.Models.Entities
{
    public class Course_Teacher
    {
        public int ID { get; set; }
        public int CourseID { get; set; }
        public string TeacherID { get; set; }
    }
}