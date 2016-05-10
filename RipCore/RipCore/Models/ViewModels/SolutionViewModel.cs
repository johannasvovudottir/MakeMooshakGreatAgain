using RipCore.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RipCore.Models.ViewModels
{
    public class SolutionViewModel
    {
        public int ID { get; set; }
        public int CurrentBestID { get; set; }
        public int MilestoneID { get; set; }
        public decimal? Grade { get; set; }
        public string StudentID { get; set; }
//<<<<<<< HEAD
//        public List<Solution> UserSubmissions;
//        public List<Solution> AllSubmissions;
//        public Submission CurrentBest { get; set; }

//=======

//        public List<Solution> UserSubmissions;
//        public List<Solution> AllSubmissions;

//        public Submission CurrentBest { get; set; }
//>>>>>>> 77512343010e2b5818de28c9403cb36c19fd7b4f
    }
}