using System;
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





        [TestMethod]
        public void CheckCourseTeacher()
        {/*
            const int id = 1;
            const int courseID = 1;
            const string TeacherID = "1";

            Course courseTeacher = personServiceTest.GetPersonById(TeacherID);

            Assert.AreEqual(id, courseTeacher.ID);
            Assert.AreEqual(courseID, courseTeacher.CourseID);
            Assert.AreEqual(TeacherID, courseTeacher.TeacherID);
            */

        }

        [TestMethod]
        public void CheckCourseStudent()
        {
            //const int id = 1;
            //const int courseID = 1;
            //const string studentID = "1";

            //Course_Student courseStudent = personServiceTest.GetPersonById(id);

            //Assert.AreEqual(id, courseStudent.ID);
            //Assert.AreEqual(courseID, courseStudent.CourseID);
            //Assert.AreEqual(studentID, courseStudent.UserID);
        }

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

        [TestMethod]
        public void CheckAdmin()
        {
            
            const string userId1 = "1";
            const string userId2 = "3";

            personServiceTest.makeAdmin(userId1);

            Assert.AreEqual(true, personServiceTest.checkIfAdmin(userId1));
            Assert.AreEqual(false, personServiceTest.checkIfAdmin(userId2));
        }

        [TestMethod]
        public void CheckMilestone()
        {
            const string title1 = "mila";
            const int id1 = 1;
            const string title2 = "samlagning";


            List<string> titles = solutionServiceTest.GetMilestoneNames(id1);

            Assert.AreEqual(title1, titles[0]);
            Assert.AreEqual(title2, titles[1]);

        }
        [TestMethod]
        public void CheckSolution()
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











    }
}

