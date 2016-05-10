using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RipCore.Models.Entities
{
    public class Solution
    {
        public int ID { get; set; }
        public int MilestoneID { get; set; }
        public string StudentID { get; set; }
        public string Code { get; set; }
        public int SubmissionID { get; set; }
        public decimal Grade { get; set; }
    }
}