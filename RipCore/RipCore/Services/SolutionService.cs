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

<<<<<<< HEAD
        public List<Tuple<string, string>> GetExpectedData(int milestoneID)
        {
            var data = (from a in db.Milestones where a.ID == milestoneID select a.TestCases).FirstOrDefault().ToString();
=======
        public List<Tuple<string, string>> GetExpectedData(int assginmentID)
        {
            var data = (from a in db.Assignments where a.ID == assginmentID select a.TestCases).FirstOrDefault().ToString();
>>>>>>> 1fe65884b1add25099cc75ace2d1a543eb62fc1c
            string[] tests = data.Split(new string[] { "\r\nTEST" }, StringSplitOptions.None);
            List<Tuple<string, string>> IOpairs = new List<Tuple<string, string>>();
            foreach (var item in tests)
            {
                string[] testPairs = item.Split(new string[] { "QUIT" }, StringSplitOptions.None);
                for (int i = 0; i < testPairs.Length; i = i + 2)
                {
                    var tmp = new Tuple<string, string>(testPairs[i], testPairs[i + 1]);
                    IOpairs.Add(tmp);
                }
            }

            return IOpairs;
        }

        public List<SubmissionViewModel> GetAllSubmissions(int milestoneID)
        {
<<<<<<< HEAD
            List<Submission> submissions = (from s in db.Submission where s.MilestoneID == milestoneID select s).ToList();
=======
            List<Submission> submissions = (from s in db.Submission where s.MilestoneID == assignmentID select s).ToList();
>>>>>>> 1fe65884b1add25099cc75ace2d1a543eb62fc1c
            List<SubmissionViewModel> submissionsViewModel = new List<SubmissionViewModel>();
            foreach (var item in submissions)
            {
                SubmissionViewModel tmp = new SubmissionViewModel { MilestoneID = item.MilestoneID, IsAccepted = item.IsAccepted, UsersName = (from n in db.Users where n.Id == item.UserID select n.UserName).FirstOrDefault() };
                submissionsViewModel.Add(tmp);
            }
            return submissionsViewModel;
        }

        public List<Submission> GetUserSubmissions(int assignmentID, string userID)
        {
            List<Submission> submissions = (from s in db.Submission
                                            where s.MilestoneID == assignmentID && s.UserID == userID
                                            select s).ToList();
            return submissions;
        }

    }
}