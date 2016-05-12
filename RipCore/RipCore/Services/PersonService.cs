using RipCore.Models;
using RipCore.Models.Entities;
using RipCore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RipCore.Services
{
    /// <summary>
    /// A service class that handles requests 
    /// regarding user data in the database
    /// </summary>
    public class PersonService
    {
        /*
        private readonly ApplicationDbContext db;

        public PersonService()
        {
            db = new ApplicationDbContext();
        }*/



        private readonly IAppDataContext db;


        public PersonService(IAppDataContext dataContext = null)
        {
            db = dataContext ?? new ApplicationDbContext();

        }
        /// <summary>
        /// A function that returns a list of all users
        /// in the database
        /// </summary>
        public List<PersonViewModel> GetAllPersons()
        {
            var Persons = db.Users.ToList();
            var viewModel = new List<PersonViewModel>();
            foreach (var item in Persons)
            {
                var temp = new PersonViewModel
                {
                    Name = item.FullName,
                    ID = item.Id
                };
                viewModel.Add(temp);
            }
            return viewModel;
        }
        /// <summary>
        /// A function that connects a student to a course in the database
        /// </summary>
        public void ConnectStudents(string userID,int courseID)
        {
               
            Course_Student newStudent = new Course_Student
            {
                UserID=userID,
                CourseID=courseID
            };
            db.CoursesStudents.Add(newStudent);
            db.SaveChanges();
            return; 
        }
        /// <summary>
        /// A function that connects a teacher to a course in the database
        /// </summary>
        public void ConnectTeachers(string userID, int courseID)
        {

            Course_Teacher newTeacher = new Course_Teacher
            {
                TeacherID = userID,
                CourseID = courseID
            };
            db.CoursesTeachers.Add(newTeacher);
            db.SaveChanges();
            return;
        }
       /// <summary>
       /// A function that returns a list of all teachers for
       /// a specific course in the database
       /// </summary>
        public List<PersonViewModel> GetAllTeachers(int courseID)
        {
            CourseService query = new CourseService();
            List<ApplicationUser> teachers = query.GetAllTeachers(courseID);
            List<PersonViewModel> teachersToReturn = new List<PersonViewModel>();
            foreach (ApplicationUser person  in teachers)
            {
                PersonViewModel personToAppend = new PersonViewModel
                {
                    ID=person.Id,
                    Email=person.Email,
                    Name=person.FullName,
                    Ssn=person.Ssn,
                    Username = person.UserName
                };
                teachersToReturn.Add(personToAppend);
            }

            return teachersToReturn;
        }
        /// <summary>
        /// A function that returns a list of all students for
        /// a specific course in the database
        /// </summary>
        public List<PersonViewModel> GetAllStudents(int courseID)
        {
            CourseService query = new CourseService();
            List<ApplicationUser> students = query.GetAllStudents(courseID);
            List<PersonViewModel> studentsToReturn = new List<PersonViewModel>();
            foreach (ApplicationUser person in students)
            {
                PersonViewModel personToAppend = new PersonViewModel
                {
                    ID = person.Id,
                    Email = person.Email,
                    Name = person.FullName,
                    Ssn = person.Ssn,
                    Username = person.UserName
                };
                studentsToReturn.Add(personToAppend);
            }

            return studentsToReturn;
        }
        /// <summary>
        /// A function that returns a list of users that are neither teachers
        /// nor students in a specific course
        /// </summary>
        public List<PersonViewModel> GetAllNotConnected(int courseID)
        {
            List<PersonViewModel> allUsers = GetAllPersons();
            List<PersonViewModel> courseTeachers = GetAllTeachers(courseID);
            List<PersonViewModel> courseStudents = GetAllStudents(courseID);
            var teacherResult = allUsers.Where(a => !courseTeachers.Any(b => b.ID == a.ID)).ToList();
            var result = teacherResult.Where(a => !courseStudents.Any(b => b.ID == a.ID)).ToList();
            
            return result;
        }
        /// <summary>
        /// A function that checks if a user does have admin rights
        /// </summary>
        public bool checkIfAdmin(string userID)
        {
            var result = (from x in db.Admins
                          where x.UserID == userID
                          select x).SingleOrDefault();
            if (result != null)
            {
                return true;
            }             

            return false;
        }
        /// <summary>
        /// A function that grants a specific user admin rights in the system
        /// </summary>
        public void makeAdmin(string userID)
        {
            Admin newAdmin = new Admin { UserID = userID };
            db.Admins.Add(newAdmin);
            db.SaveChanges();
        }
        /// <summary>
        /// A function that removes admin rights from a specific user in the system
        /// </summary>
        public void removeAdmin(string userID)
        {
            Admin adminToRemove = (from x in db.Admins
                                   where x.UserID == userID
                                   select x).SingleOrDefault();
            db.Admins.Remove(adminToRemove);
            db.SaveChanges();
        }
        /// <summary>
        /// A function that returns a single person from the database
        /// given a specific ID
        /// </summary>
        public PersonViewModel GetPersonById(string personID)
        {
            var result = (from x in db.Users
                          where x.Id == personID
                          select x).SingleOrDefault();

            if (result == null)
            {
                //TODO kasta villu
                return null;
            }

            var viewModel = new PersonViewModel
            {
                Name = result.FullName,
                ID = result.Id,
                Ssn = result.Ssn,
                PasswordHash = result.PasswordHash,
                Email = result.Email,
                Username = result.UserName,
                CentrisUser = result.CentrisUser
            };

            return viewModel;
        }
    }
}