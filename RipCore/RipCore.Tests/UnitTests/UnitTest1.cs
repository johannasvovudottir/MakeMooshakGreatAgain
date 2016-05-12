using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RipCore.Services;
using RipCore.Models.Entities;
using RipCore.Models;
using RipCore.Models.ViewModels;

namespace RipCore.Tests.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        private PersonService personServiceTest;
        private AssignmentsService assignmentServiceTest;
        private CourseService courseServiceTest;


        [TestInitialize]
        public void Initialize()
        {
            MockDataContext mock = new MockDataContext();





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
                CourseID = 3,
                TeacherID = "3"
            };
            Course_Teacher courseTeacher4 = new Course_Teacher
            {
                ID = 4,
                CourseID = 4,
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
                CourseID = 3,
                UserID = "3"
            };
            Course_Student courseStudent4 = new Course_Student
            {
                ID = 4,
                CourseID = 4,
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
            #region Admins



            #endregion



            personServiceTest = new PersonService(mock);
            assignmentServiceTest = new AssignmentsService(mock);
            courseServiceTest = new CourseService(mock);

        }



        [TestMethod]
        public void CheckPerson()
        {
            /*
            const int id = 1;
            const string FullName = "Olafur Valur Valdimarsson";
            const string UserName = "oli kantur";
            const int Ssn = 0811903459;
            const string Email = "olikantur@gmail.com";
            const string Password = "Olikantur1!";
            const string Passkey = "babling";

            AppUser appUser = personServiceTest.GetPersonById(id);

            Assert.AreEqual(id, appUser.ID);
            Assert.AreEqual(UserName, appUser.UserName);
            Assert.AreEqual(FullName, appUser.FullName);
            Assert.AreEqual(Ssn, appUser.Ssn);
            Assert.AreEqual(Email, appUser.Email);
            Assert.AreEqual(Password, appUser.Password);
            Assert.AreEqual(Passkey, appUser.Passkey);
            */

            //const string FullName = "Olafur Valur Valdimarsson";
            //const int Ssn = 0811903459;
            //const bool CentrisUser = true;



        }

        [TestMethod]
        public void CheckAssignment()
        {
            const int id = 1;
            const string title = "HelloWorld";
            const int weight = 30;
            const int courseID = 1;
            const string testCases = "swag";
            const string description = "Forrita Hello World!";
            const int programmingLanguageID = 1;

            AssignmentViewModel assignment = assignmentServiceTest.GetAssignmentsById(id);

            Assert.AreEqual(id, assignment.ID);
            Assert.AreEqual(title, assignment.Title);
            Assert.AreEqual(weight, assignment.Weight);
            Assert.AreEqual(courseID, assignment.CourseID);
            Assert.AreEqual(testCases, assignment.TestCases);
            Assert.AreEqual(description, assignment.Description);
            Assert.AreEqual(programmingLanguageID, assignment.ProgrammingLanguageID);



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
            personServiceTest.ConnectStudents("1", 1);
            personServiceTest.ConnectStudents("2", 1);
            personServiceTest.ConnectStudents("3", 1);
            personServiceTest.ConnectStudents("1", 2);
            personServiceTest.ConnectTeachers("4", 1);
            personServiceTest.ConnectTeachers("3", 2);

            var courses = courseServiceTest.GetCoursesWhereTeacher("4");

        }






    }
}

