﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RipCore.Services;
using RipCore.Models.Entities;
using RipCore.Models;
using RipCore.Models.ViewModels;
using System.Collections.Generic;

namespace RipCore.Tests.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        private PersonService personServiceTest;
        private AssignmentsService assignmentServiceTest;
        private CourseService courseServiceTest;
        private SolutionService solutionServiceTest;

        /// <summary>
        /// Initialize instances of all the classes in Mock Database
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            MockDataContext mock = new MockDataContext();


            #region Users
            ApplicationUser user1 = new ApplicationUser
            {
                Id = "1",
                Ssn = "1211",
                Email = "maggi@prins.is",
                UserName = "HárbustaMaggi"
                
            };

            ApplicationUser user2 = new ApplicationUser
            {
                Id = "2",
                Ssn = "2122",
                Email = "gunnar@Dr.is",
                UserName = "DrGunni"
            };

            ApplicationUser user3 = new ApplicationUser
            {
                Id = "3",
                Ssn = "3102",
                Email = "valgeir@pipar.is",
                UserName = "vallisport"
            };
            ApplicationUser user4 = new ApplicationUser
            {
                Id = "4",
                Ssn = "3789",
                Email = "siggi@gmail.com",
                UserName = "siggiskor"
            };
            mock.Users.Add(user1);
            mock.Users.Add(user2);
            mock.Users.Add(user3);
            mock.Users.Add(user4);
            #endregion
            #region Assignments




            Assignment assignment1 = new Assignment
            {
                ID = 1,
                Title = "HelloWorld",
                Weight = 30,
                CourseID = 1,
                TestCases = "swag",
                Description = "Forrita Hello World!",
                ProgrammingLanguageID = 1,
                DateCreated = new DateTime(2016, 6, 20),
                DueDate = new DateTime(2016, 6, 30)


            };
            mock.Assignments.Add(assignment1);
            #endregion
            #region Milestones
                        Milestone milestone1 = new Milestone
            {
                ID = 1,
                AssignmentID = 1,
                Weight = 20,
                Title = "mila",
                Description = "mæla milu",
                Input = "km",
                Output = "milan",
                Code = "1km",
                TestCases = "profa km",
                ProgrammingLanguageID = 1,
                DateCreated = new DateTime(2016, 4, 20),
                DueDate = new DateTime(2016, 4, 30)

            };
            Milestone milestone2 = new Milestone
            {
                ID = 2,
                AssignmentID = 1,
                Weight = 25,
                Title = "samlagning",
                Description = "leggja saman tölur",
                Input = "2,8",
                Output = "10",
                Code = "cout << 2 + 8;",
                TestCases = "profa tolur",
                ProgrammingLanguageID = 2,
                DateCreated = new DateTime(2016, 5, 20),
                DueDate = new DateTime(2016, 5, 30)

            };
            Milestone milestone3 = new Milestone
            {
                ID = 3,
                AssignmentID = 2,
                Weight = 25,
                Title = "samlagning",
                Description = "leggja saman tölur",
                Input = "2,4",
                Output = "6",
                Code = "cout << 2 + 4;",
                TestCases = "profa tolur",
                ProgrammingLanguageID = 2,
                DateCreated = new DateTime(2016, 3, 20),
                DueDate = new DateTime(2016, 3, 30)

            };
            mock.Milestones.Add(milestone1);
            mock.Milestones.Add(milestone2);
            mock.Milestones.Add(milestone3);


            #endregion
            #region CourseTeachers

            Course_Teacher courseTeacher1 = new Course_Teacher
            {
                ID = 1,
                CourseID = 1,
                TeacherID = "1"
            };
            Course_Teacher courseTeacher2 = new Course_Teacher
            {
                ID = 2,
                CourseID = 2,
                TeacherID = "2"
            };
            Course_Teacher courseTeacher3 = new Course_Teacher
            {
                ID = 3,
                CourseID = 1,
                TeacherID = "3"
            };
            Course_Teacher courseTeacher4 = new Course_Teacher
            {
                ID = 4,
                CourseID = 1,
                TeacherID = "4"
            };
            mock.CoursesTeachers.Add(courseTeacher1);
            mock.CoursesTeachers.Add(courseTeacher2);
            mock.CoursesTeachers.Add(courseTeacher3);
            mock.CoursesTeachers.Add(courseTeacher4);
            #endregion
            #region CourseStudents

            Course_Student courseStudent1 = new Course_Student
            {
                ID = 1,
                CourseID = 1,
                UserID = "1"
            };
            Course_Student courseStudent2 = new Course_Student
            {
                ID = 2,
                CourseID = 2,
                UserID = "2"
            };
            Course_Student courseStudent3 = new Course_Student
            {
                ID = 3,
                CourseID = 2,
                UserID = "3"
            };
            Course_Student courseStudent4 = new Course_Student
            {
                ID = 4,
                CourseID = 1,
                UserID = "4"
            };

            mock.CoursesStudents.Add(courseStudent1);
            mock.CoursesStudents.Add(courseStudent2);
            mock.CoursesStudents.Add(courseStudent3);
            mock.CoursesStudents.Add(courseStudent4);


            #endregion
            #region Courses

            Course course1 = new Course
            {
                ID = 1,
                Name = "Byrjenda Forritun",
                SchoolID = 1,
                Semester = "Vorönn",
                Year = 2013
            };
            Course course2 = new Course
            {
                ID = 2,
                Name = "Leikjaorritun",
                SchoolID = 2,
                Semester = "Haustönn",
                Year = 2018
            };

            mock.Courses.Add(course1);
            mock.Courses.Add(course2);

            #endregion
            #region Solutions
            Solution solution1 = new Solution
            {
                ID = 1,
                MilestoneID = 1,
                StudentID = "1",
                Code = "Hello World",
                SubmissionID = 1,
                Grade = 6.7M
            };
            Solution solution2 = new Solution
            {
                ID = 2,
                MilestoneID = 3,
                StudentID = "2",
                Code = "Hello Universe",
                SubmissionID = 2,
                Grade = 7.9M
            };
            mock.Solutions.Add(solution1);
            mock.Solutions.Add(solution2);

            #endregion
            #region Submissions
            Submission submission1 = new Submission
            {
                ID = 1,
                UserID = "1",
                MilestoneID = 1,
                IsAccepted = true,
                SolutionOutput = "Hello World",
                ExpectedOutput = "Hello World",
                Code = "cout << Hello World "
            };
            mock.Submission.Add(submission1);
            #endregion
            #region Admins

            Admin admin1 = new Admin
            {
                ID = 1,
                UserID = "1"
            };
            Admin admin2 = new Admin
            {
                ID = 2,
                UserID = "2"
            };


            #endregion



            personServiceTest = new PersonService(mock);
            assignmentServiceTest = new AssignmentsService(mock);
            courseServiceTest = new CourseService(mock);
            solutionServiceTest = new SolutionService(mock);

        }

        /// <summary>
        /// checks if attributes in Mock Database user matches with constant that are the same 
        /// </summary>
        [TestMethod]
        public void CheckPerson()
        {
            
            const string id = "1";
            const string userName = "HárbustaMaggi";
            const string ssn = "1211";
            const string email = "maggi@prins.is";

            var user = personServiceTest.GetPersonById(id);

            Assert.AreEqual(id, user.ID);
            Assert.AreEqual(userName, user.Username);
            Assert.AreEqual(ssn, user.Ssn);
            Assert.AreEqual(email, user.Email);

        }

        /// <summary>
        ///  checks if attributes in Mock Database Course matches with constant that are the same
        /// </summary>
        [TestMethod]
        public void CheckCourse()
        {
            const int id = 1;
            const string name = "Byrjenda Forritun";
            const string semester = "Vorönn";
            const int year = 2013;

            var course = courseServiceTest.GetCourseByID(id);

            Assert.AreEqual(id, course.ID);
            Assert.AreEqual(name, course.Name);
            Assert.AreEqual(semester, course.Semester);
            Assert.AreEqual(year, course.Year);


        }

        /// <summary>
        /// checks if number of teacher and students in each Course matches with Mock Database 
        /// </summary>
        [TestMethod]
        public void CheckTeachersAndStudents()
        {
            const int courseId1 = 1;
            const int courseId2 = 2;
            const int course1TeachersCount = 3;
            const int course2TeachersCount = 1;
            const int course1StudentsCount = 2;
            const int course2StudentsCount = 2;

            List<ApplicationUser> course1Students = courseServiceTest.GetAllStudents(courseId1);
            List<ApplicationUser> course1Teachers = courseServiceTest.GetAllTeachers(courseId1);
            List<ApplicationUser> course2Students = courseServiceTest.GetAllStudents(courseId2);
            List<ApplicationUser> course2Teachers = courseServiceTest.GetAllTeachers(courseId2);

            Assert.AreEqual(course1TeachersCount, course1Teachers.Count);
            Assert.AreEqual(course2TeachersCount, course2Teachers.Count);
            Assert.AreEqual(course1StudentsCount, course1Students.Count);
            Assert.AreEqual(course2StudentsCount, course2Students.Count);
        }

        /// <summary>
        /// checks connection between courses in Mock Database
        /// </summary>
        [TestMethod]
        public void CheckCourseConnection()
        {
            

            const int courseId1 = 1;
            const int courseId2 = 2;
            const string userId1 = "1";
            const string userId2 = "2";
            const string userId4 = "4";
            const string teacherId1 = "1";
            const string teacherId3 = "3";
            const int courseCount1 = 1;
            const int courseCount2 = 2;
            personServiceTest.ConnectStudents(userId1, courseId2);
            personServiceTest.ConnectStudents(userId2, courseId1);
            personServiceTest.ConnectTeachers(teacherId1, courseId2);
            

            List<Course> courses1 = courseServiceTest.GetCoursesWhereStudent(userId1);
            List<Course> courses2 = courseServiceTest.GetCoursesWhereStudent(userId2);
            List<Course> courses3 = courseServiceTest.GetCoursesWhereStudent(userId4);
            List<Course> courses4 = courseServiceTest.GetCoursesWhereTeacher(teacherId1);
            List<Course> courses5 = courseServiceTest.GetCoursesWhereTeacher(teacherId3);

            Assert.AreEqual(courseCount2, courses1.Count);
            Assert.AreEqual(courseCount2, courses2.Count);
            Assert.AreEqual(courseCount1, courses3.Count);
            Assert.AreEqual(courseCount2, courses4.Count);
            Assert.AreEqual(courseCount1, courses5.Count);

            Assert.AreEqual(courseId1, courses1[0].ID);
            Assert.AreEqual(courseId2, courses1[1].ID);
            Assert.AreEqual(courseId1, courses5[0].ID);
        }

        /// <summary>
        ///  checks if attributes in Mock Database Admin matches with constant that are the same
        /// </summary>
        [TestMethod]
        public void CheckAdmin()
        {
            
            const string userId1 = "1";
            const string userId2 = "3";

            personServiceTest.makeAdmin(userId1);

            Assert.AreEqual(true, personServiceTest.checkIfAdmin(userId1));
            Assert.AreEqual(false, personServiceTest.checkIfAdmin(userId2));
        }

        /// <summary>
        ///  checks if attributes in Mock Database Milestone matches with constant that are the same
        /// </summary>
        [TestMethod]
        public void CheckMilestone()
        {
            const string title1 = "mila";
            const int id1 = 1;
            const int assignmentId1 = 1;
            const string title2 = "samlagning";


            List<Milestone> milestone = solutionServiceTest.GetMilestoneNames(id1);

            Assert.AreEqual(title1, milestone[0].Title);
            Assert.AreEqual(title2, milestone[1].Title);
            Assert.AreEqual(id1, milestone[0].ID);
            Assert.AreEqual(assignmentId1, milestone[0].AssignmentID);
            

        }

        /// <summary>
        /// checks connection between Solutions and Milestones in Mock Database
        /// </summary>
        [TestMethod]
        public void CheckSolutionAndMilestones()
        {
            const string userId1 = "1";
            const string userId2 = "2";
            const int assignmentId1 = 1;
            const int assignmentId2 = 2;

            List<Solution> solutions1 = assignmentServiceTest.GetSolutionsById(userId1, assignmentId1);
            List<Solution> solutions2 = assignmentServiceTest.GetSolutionsById(userId2, assignmentId2);



            Milestone milestone1 = assignmentServiceTest.GetMilestoneBySolution(solutions1[0]);
            Milestone milestone2 = assignmentServiceTest.GetMilestoneBySolution(solutions2[0]);

            Assert.AreEqual(milestone1.ID, solutions1[0].MilestoneID);
            Assert.AreEqual(milestone2.ID, solutions2[0].MilestoneID);

            Assert.AreEqual(userId1, solutions1[0].StudentID);
            Assert.AreEqual(userId2, solutions2[0].StudentID);

        }

        /// <summary>
        ///  checks if attributes in Mock Database Submission matches with constant that are the same
        /// </summary>
        [TestMethod]
        public void CheckSubmission()
        {
            const string userId1 = "1";
            const int assignmentId1 = 1;
            const int submissionId1 = 1;
            const int count = 1;
            

            List<Submission> submissions1 = solutionServiceTest.GetUserSubmissions(assignmentId1, userId1);

            Assert.AreEqual(count, submissions1.Count);
            Assert.AreEqual(userId1, submissions1[0].UserID);
            Assert.AreEqual(submissionId1, submissions1[0].ID);
        }

    }
}

