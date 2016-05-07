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

        public List<Tuple<string, string>> GetExpectedData(int assgintmentID)
        {
            var data = (from a in db.Assignments where a.ID == assgintmentID select a.Input).FirstOrDefault().ToString();
            string[] result = data.Split(new string[] { "QUIT" }, StringSplitOptions.None);
            List<Tuple<string, string>> IOpairs = new List<Tuple<string, string>>();
            for (int i = 0; i < result.Length; i=i+2)
            {
                var tmp = new Tuple<string, string>(result[i], result[i + 1]);
                IOpairs.Add(tmp);
            }
            return IOpairs;
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