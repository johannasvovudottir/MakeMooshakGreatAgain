﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RipCore.Models.ViewModels
{
    public class AssignmentViewModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int Weight { get; set; }
        public string Description { get; set; }
        public int CourseID { get; set; }
        public List<AssignmentMilestoneViewModel> milestones;
        //[Required, Microsoft.Web.Mvc.FileExtensions(Extensions = "csv",
        // ErrorMessage = "Specify a CSV file. (Comma-separated values)")]
        //public byte[] DescriptionFile { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DueDate { get; set; }

        // List<string> input;
        // List<string> output;
    }
}