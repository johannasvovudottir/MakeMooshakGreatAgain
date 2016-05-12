﻿using RipCore.Models;
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
        /*
        private ApplicationDbContext db;
        public SolutionService()
        {
            db = new ApplicationDbContext();
        }*/

        private readonly IAppDataContext db;

        public SolutionService(IAppDataContext dataContext = null)
        {
            db = dataContext ?? new ApplicationDbContext();
        }

        public List<Tuple<string, string>> GetExpectedData(int milestoneID)
        {
            var data = (from a in db.Milestones where a.ID == milestoneID select a.TestCases).FirstOrDefault().ToString();

            string[] tests = data.Split(new string[] { "\r\nTEST\r\n" }, StringSplitOptions.None);
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

        public SubmissionViewModel GetSubmissionForView(int id)
        {
            Submission submission = (from s in db.Submission
                          where s.ID == id
                          select s).FirstOrDefault();
            SubmissionViewModel viewModel = new SubmissionViewModel { ID = id, MilestoneID = submission.MilestoneID, MilestoneName = (from m in db.Milestones where m.ID == submission.MilestoneID select m.Title).FirstOrDefault().ToString(), IsAccepted = submission.IsAccepted, Code = submission.Code, SolutionOutput = submission.SolutionOutput, ExpectedOutput = submission.ExpectedOutput };
            return viewModel;
        }

        public List<List<SubmissionViewModel>> GetSubmissionsForUser(int id, string userID)
        {
            List<int> milestoneIDs = (from m in db.Milestones where m.AssignmentID == id select m.ID).ToList();
            List<List<SubmissionViewModel>> submissions = new List<List<SubmissionViewModel>>();
            foreach (var item in milestoneIDs)
            {
                List<Submission> tmp = (from s in db.Submission where s.MilestoneID == item && s.UserID == userID select s).ToList();
                List<SubmissionViewModel> submissionsList = new List<SubmissionViewModel>();
                foreach (var i in tmp)
                {
                    SubmissionViewModel newViewModel = new SubmissionViewModel { ID = i.ID, MilestoneID = i.MilestoneID, UsersID = i.UserID, UsersName = (from u in db.Users where u.Id == userID select u.UserName).FirstOrDefault() };
                    submissionsList.Add(newViewModel);
                }
                submissions.Add(submissionsList);
            }
            return submissions;
        }

        public List<string> GetMilestoneNames(int id)
        {
            List<string> names = (from m in db.Milestones
                                     where m.AssignmentID == id
                                     select m.Title).ToList();
            return names;
        }

        public List<List<SubmissionViewModel>> GetAllNotConnected(int assignmentID, string userID)
        {
            List<List<SubmissionViewModel>> otherStudents = new List<List<SubmissionViewModel>>();
            List<int> milestoneIDs = (from m in db.Milestones where m.AssignmentID == assignmentID select m.ID).ToList();
            foreach (var item in milestoneIDs)
            {
                List<SubmissionViewModel> allUserSubmissions = GetUsersSubmissions(item, userID);
                List<SubmissionViewModel> allSubmissions = GetAllSubmissions(item);
                List<SubmissionViewModel> result = allSubmissions.Where(a => !allUserSubmissions.Any(b => b.ID == a.ID)).ToList();
                otherStudents.Add(result);
            }

            return otherStudents;
        }

        public List<List<SolutionViewModel>> GetSolutionsForView(int id)
        {
            List<int> milestoneIDs = (from m in db.Milestones where m.AssignmentID == id select m.ID).ToList();
            List<List<SolutionViewModel>> solutions = new List<List<SolutionViewModel>>();
            foreach (var item in milestoneIDs)
            {
                List<Solution> tmp = (from s in db.Solutions where s.MilestoneID == item select s).ToList();
                List<SolutionViewModel> solutionsList = new List<SolutionViewModel>();
                foreach(var i in tmp)
                {
                    SolutionViewModel newViewModel = new SolutionViewModel { ID = i.ID, CurrentBestID = i.SubmissionID, Grade = i.Grade, MilestoneID = i.MilestoneID, StudentID = i.StudentID };
                    solutionsList.Add(newViewModel);
                }
                solutions.Add(solutionsList);
            }
            return solutions;
        }

        public List<SubmissionViewModel> GetUsersSubmissions(int milestoneID, string userID)
        {
            List<Submission> allUserSubmissions = (from m in db.Submission where m.MilestoneID == milestoneID && m.UserID == userID select m).ToList();
            List<SubmissionViewModel> viewModel = new List<SubmissionViewModel>();
            foreach (var item in allUserSubmissions)
            {
                SubmissionViewModel tmp = new SubmissionViewModel { ID = item.ID, IsAccepted = item.IsAccepted, Code = item.Code, ExpectedOutput = item.ExpectedOutput, MilestoneID = item.MilestoneID, SolutionOutput = item.SolutionOutput, UsersName = (from u in db.Users where u.Id == item.UserID select u.UserName).FirstOrDefault(), MilestoneName = (from m in db.Milestones where m.ID == item.MilestoneID select m.Title).FirstOrDefault() };
                viewModel.Add(tmp);
            }
            return viewModel;
        }

        public List<Submission> GetUserSubmissions(int assignmentID, string userID)
        {
            List<Submission> submissions = (from s in db.Submission
                                            where s.MilestoneID == assignmentID && s.UserID == userID
                                            select s).ToList();
            return submissions;
        }


        public Solution GetBestSubmissionByID(int milestoneID, string StudentID)
        {
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

        public List<List<string>> GetExpectedRegex(int milestoneID)
        {
            var data = (from a in db.Milestones where a.ID == milestoneID select a.TestCases).FirstOrDefault().ToString();
            string[] tests = data.Split(new string[] { "\r\nNOT\r\n" }, StringSplitOptions.None);
            List<List<string>> strings = new List<List<string>>();
            string[] acceptingStrings = tests[0].Split(new string[] { "\r\n" }, StringSplitOptions.None);
            List<string> tmp = new List<string>();
            foreach (var item in acceptingStrings)
            {
                tmp.Add(item);
            }
            string[] nonAcceptingStrings = tests[1].Split(new string[] { "\r\n" }, StringSplitOptions.None);
            List<string> temp = new List<string>();
            foreach (var item in nonAcceptingStrings)
            {
                temp.Add(item);
            }
            strings.Add(tmp);
            strings.Add(temp);
            return strings;
        }

        public string GetTestCase(int milestoneID)
        {
            string expectedOutput = (from m in db.Milestones where m.ID == milestoneID select m.TestCases).ToString();
            return expectedOutput;
        }
    }
}