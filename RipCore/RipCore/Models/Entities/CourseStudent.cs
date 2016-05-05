using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RipCore.Models.Entities
{
    public class Course_Student
    {
        public int ID { get; set; }
        public int CourseID { get; set; }
        public string UserID { get; set; }
    }
}