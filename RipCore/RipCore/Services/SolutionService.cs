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

        public List<Tuple<string, string>> GetExpectedData(int milestoneID)
        {
            var data = (from a in db.Milestones where a.ID == milestoneID select a.TestCases).FirstOrDefault().ToString();

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
            List<Submission> submissions = (from s in db.Submission where s.MilestoneID == milestoneID select s).ToList();
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

        public Solution GetBestSubmissionByID(int milestoneID, string StudentID) {
            var result = (from s in db.Solutions
                          where s.MilestoneID == milestoneID && s.StudentID == StudentID
                          select s).LastOrDefault();

            return result; 
        }

        public Submission GetSubmissionByID(int submissionID)
        {
            var result = (from s in db.Submission
                          where s.ID == submissionID
                          select s).LastOrDefault();

            return result;
        }
        //spyrja valbjorn afh database uppfaerist ekki

    }
}