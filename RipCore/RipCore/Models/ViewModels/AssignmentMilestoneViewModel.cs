using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RipCore.Models.ViewModels
{
    public class AssignmentMilestoneViewModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Weight { get; set; }
        public int AssignmentID { get; set; }
        public HttpPostedFileBase File { get; set; }
    }
}