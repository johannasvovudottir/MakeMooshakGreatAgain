using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RipCore.Models.ViewModels
{
    public class SubmissionViewModel
    {
        public int ID { get; set; }
        public string UsersName { get; set; }
        public string UsersID { get; set; }
        public string AssignmentName { get; set; }
        public string MilestoneName { get; set; }
        public int MilestoneID { get; set; }
        public bool IsAccepted { get; set; }
        public string SolutionOutput { get; set; }
        public string Code { get; set; }
        public string ExpectedOutput { get; set; }
        public string ProgrammingLanguage { get; set; }
    }
}