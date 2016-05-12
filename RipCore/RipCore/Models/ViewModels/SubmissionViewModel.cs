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
        public bool IsTeacher { get; set; }
        public List<string> SolutionOutput;
        public decimal Grade { get; set; }
        public string Code { get; set; }
        public List<string> ExpectedOutput;
        public string ProgrammingLanguage { get; set; }
    }
}