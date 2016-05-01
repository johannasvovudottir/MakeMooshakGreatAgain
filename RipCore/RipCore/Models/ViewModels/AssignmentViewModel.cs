﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RipCore.Models.ViewModels
{
    public class AssignmentViewModel
    {
        public string Title { get; set; }
        public int Weight { get; set; }
        public string Description { get; set; }
        public List<AssignmentMilestoneViewModel> milestones;
        public DateTime DateCreated { get; set; }
        public DateTime DueDate { get; set; }

        // List<string> input;
        // List<string> output;
    }
}