using RipCore.Models;
using RipCore.Models.Entities;
using RipCore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RipCore.Services
{
    public class SolutionService
    {
        private ApplicationDbContext db;
        public SolutionService()
        {
            db = new ApplicationDbContext();
        }

        public List<Submission> GetAllSubmissions(int assignmentID)
        {
            List<Submission> submission = (from s in db.Submission where s.AssignmentID == assignmentID select s).ToList();
            return submission;
        }

        public List<Submission> GetUserSubmissions(int assignmentID, string userID)
        {
            List<Submission> submissions = (from s in db.Submission
                               where s.AssignmentID == assignmentID && s.UserID == userID select s).ToList();
            return submissions;
        }

    }
}