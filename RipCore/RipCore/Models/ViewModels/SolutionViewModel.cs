﻿using RipCore.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RipCore.Models.ViewModels
{
    public class SolutionViewModel
    {
        public int ID { get; set; }
        public int AssignmentID { get; set; }
        public string StudentID { get; set; }
        public List<Submission> submissions;

    }
}