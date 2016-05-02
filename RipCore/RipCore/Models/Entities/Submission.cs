using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RipCore.Models.Entities
{
    public class Submission
    {
        public int ID { get; set; }
        public int SolutionID { get; set; }
        public string FileContent { get; set; }
        public bool IsAccepted { get; set; }
    }
}