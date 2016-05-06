using RipCore.Models.ViewModels;
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
        public int Weight { get; set; }
        public int CourseID { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public string Description { get; set; }
        public int ProgrammingLanguageID { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DueDate { get; set; }
    }
}