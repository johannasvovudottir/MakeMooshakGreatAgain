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
            List<int> submissionsIDs = (from s in db.Solutions where s.MilestoneID == milestoneID select s.SubmissionID).ToList();
            List<SubmissionViewModel> submissionsViewModel = new List<SubmissionViewModel>();
            foreach (var item in submissionsIDs)
            {
                Submission submission = (from s in db.Submission where s.ID == item select s).FirstOrDefault();
                SubmissionViewModel tmp = new SubmissionViewModel { MilestoneID = submission.MilestoneID, IsAccepted = submission.IsAccepted, UsersName = (from n in db.Users where n.Id == submission.UserID select n.UserName).FirstOrDefault() };
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

        public SubmissionViewModel GetSubmissionForView(int id)
        {
            Submission submission = (from s in db.Submission
                          where s.ID == id
                          select s).FirstOrDefault();
            SubmissionViewModel viewModel = new SubmissionViewModel { ID = id, MilestoneID = submission.MilestoneID, MilestoneName = (from m in db.Milestones where m.ID == submission.MilestoneID select m.Title).FirstOrDefault().ToString(), IsAccepted = submission.IsAccepted };
            return viewModel;
        }
        //spyrja valbjorn afh database uppfaerist ekki


        public Solution GetBestSubmissionByID(int milestoneID, string StudentID) {
            var result = (from s in db.Solutions
                          where s.MilestoneID == milestoneID && s.StudentID == StudentID
                          orderby s.ID descending
                          select s).FirstOrDefault();

            return result; 
        }

        public Submission GetSubmissionByID(int submissionID)
        {
            var result = (from s in db.Submission
                          where s.ID == submissionID
                          orderby s.ID descending
                          select s).FirstOrDefault();

            return result;
        }
        //spyrja valbjorn afh database uppfaerist ekki


    }
}