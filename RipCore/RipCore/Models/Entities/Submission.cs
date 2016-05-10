using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RipCore.Models.Entities
{
    public class Submission
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public int MilestoneID { get; set; }
        public bool IsAccepted { get; set; }
        public string SolutionOutput { get; set; }
        public string Code { get; set; }
    }
}