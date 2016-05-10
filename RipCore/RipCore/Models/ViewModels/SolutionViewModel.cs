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
        public int MilestoneID { get; set; }
        public string StudentID { get; set; }
<<<<<<< HEAD
=======
        public List<Solution> UserSubmissions;
        public List<Solution> AllSubmissions;
>>>>>>> 5b866b70930d72ca957efa3d880b1e1d0085505c
        public Submission CurrentBest { get; set; }
    }
}