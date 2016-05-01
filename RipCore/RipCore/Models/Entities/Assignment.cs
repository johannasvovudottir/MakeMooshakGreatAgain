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
        //List<string> input;
        //List<string> output;
        public string Description { get; set; }
        //public List<AssignmentMilestoneViewModel> milestones;
        public DateTime DateCreated { get; set; }
        public DateTime DueDate { get; set; }
    }
}