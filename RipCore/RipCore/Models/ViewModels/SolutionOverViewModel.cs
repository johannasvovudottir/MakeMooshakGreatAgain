using RipCore.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RipCore.Models.ViewModels
{
    public class SolutionOverViewModel
    {
        public List<List<SolutionViewModel>> AssignmentSolutions;
        public List<Milestone> MilestoneNames;
    }
}