using RipCore.Models;
using RipCore.Models.Entities;
using RipCore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RipCore.Services
{
    public class PersonService
    {
        private ApplicationDbContext db;

        public PersonService()
        {
            db = new ApplicationDbContext();
        }

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
        public List<PersonViewModel> GetAllNotConnected(int courseID)
        {
            List<PersonViewModel> allUsers = GetAllPersons();
            List<PersonViewModel> courseTeachers = GetAllTeachers(courseID);
            List<PersonViewModel> courseStudents = GetAllStudents(courseID);
            //var noteachersallowedher = from c in db.Courses join cn in db.CoursesTeachers on c.ID equals cn.TeacherID select !c
            //for (int i = 0; i < allUsers.Count; i++)
            //{
            //    for (int j = 0; j < courseTeachers.Count; j++)
            //    {
            //        if (allUsers[i].ID == courseTeachers[j].ID)
            //        {
            //            allUsers.RemoveAt(i);
            //                i++;
            //        }
            //    }
            //}
            //for (int i = 0; i < allUsers.Count; i++)
            //{
            //        for (int j = 0; j < courseStudents.Count; j++)
            //        {
            //            if (allUsers[i].ID == courseStudents[j].ID)
            //            {
            //                allUsers.RemoveAt(i);
            //                i++;
            //            }

            //    }

            //}

            return allUsers;
        }
        public PersonViewModel GetPersonById(string PersonID)
        {
            var result = (from x in db.Users
                          where x.Id == PersonID
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
                Username = result.UserName
            };

            return viewModel;
        }
    }
}