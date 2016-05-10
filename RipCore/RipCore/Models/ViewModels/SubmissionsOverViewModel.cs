using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RipCore.Models.ViewModels
{
    public class SubmissionsOverViewModel
    {
        public List<List<SubmissionViewModel>> otherSubmissions;
        public List<List<SubmissionViewModel>> usersSubmissions;
        public List<string> MilestoneNames;
    }
}