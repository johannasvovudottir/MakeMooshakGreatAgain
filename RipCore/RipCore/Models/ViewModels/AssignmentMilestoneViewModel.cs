using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RipCore.Models.ViewModels
{
    public class AssignmentMilestoneViewModel
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Efnisþátturinn verður að hafa titil!")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Efnisþátturinn verður að hafa lýsingu!")]
        public string Description { get; set; }
        [Range(0, 100, ErrorMessage = "Vægi efnisþátts verður að vera á bilinu 0-100")]
        public int Weight { get; set; }
        public int AssignmentID { get; set; }
        public HttpPostedFileBase File { get; set; }
        public string TestCases { get; set; }
    }
}