using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RipCore.Models.ViewModels
{
    public class AssignmentViewModel
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public int CourseID { get; set; }
        public int Weight { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Solution { get; set; }
        public List<AssignmentMilestoneViewModel> Milestones;
        public bool IsTeacher { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DueDate { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public HttpPostedFileBase File { get; set; }
    }
}