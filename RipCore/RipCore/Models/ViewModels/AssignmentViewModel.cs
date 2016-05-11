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
        public string CourseName { get; set; }
        [Range(0, 100, ErrorMessage = "Project weight must be in range 0-100")]
        public int Weight { get; set; }
        public int ProgrammingLanguageID { get; set; }
        [Required(ErrorMessage = "You must specify a title!")]
        public string Title { get; set; }
        [Required(ErrorMessage = "You must add a Project description!")]
        public string Description { get; set; }
        public string Solution { get; set; }
        public List<AssignmentMilestoneViewModel> Milestones;
        public List<SelectListItem> programmingLanguages;
        public List<SelectListItem> milestoneNumber;
        public int milestoneSubmissionID { get; set; }
        public bool IsTeacher { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DueDate { get; set; }
        public string TestCases { get; set; }
        public HttpPostedFileBase File { get; set; }
        public int NumberOfHandins { get; set; }
        public int NumberOfStudents { get; set; }
    }
}