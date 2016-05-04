using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RipCore.Models.Entities
{
    public class Course
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int SchoolID { get; set; }
        public string Semester { get; set; }
        public int Year { get; set; }

        /*TODO
        students
        techers
        assignment*/
    }
}