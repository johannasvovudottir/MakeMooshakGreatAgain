using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RipCore.Models.Entities
{
    public class Assignment
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Veight { get; set; }
        public int CourseID { get; set; }
        List<string> input;
        List<string> output;
    }
}