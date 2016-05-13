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
        [Range(0, 100, ErrorMessage = "Vægi verkefnis verður að vera á bilinu 0-100")]
        public int Weight { get; set; }
        public double Grade { get; set; }
        public int ProgrammingLanguageID { get; set; }
        [Required(ErrorMessage = "Verkefnið verður að hafa titil!")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Verkefnið verður að hafa lýsingu!")]
        public string Description { get; set; }
        public string Solution { get; set; }
        public List<AssignmentMilestoneViewModel> Milestones;
        public List<SelectListItem> programmingLanguages;
        public List<SelectListItem> milestoneNumber;
        public int milestoneSubmissionID { get; set; }
        public bool IsTeacher { get; set; }
        public DateTime DateCreated { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime DueDate { get; set; }
        public string TestCases { get; set; }
        public HttpPostedFileBase File { get; set; }
        public int NumberOfHandins { get; set; }
        public int NumberOfNotHandedIn { get; set; }
        [Required(ErrorMessage = "Verkefnið verður að hafa skilgreint forritunarmál!")]
        public string ProgrammingLanguage { get; set; }

    }
}
