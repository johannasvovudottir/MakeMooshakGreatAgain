using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RipCore.Models.Entities
{
    public class Solution
    {
        public int ID { get; set; }
        public int AssignmentID { get; set; }
        public string StudentID { get; set; }
        public string SolutionOutput { get; set; }


    }
}