﻿using RipCore.Models;
using RipCore.Models.Entities;
using RipCore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RipCore.Services
{
    public class CourseService
    {
        private ApplicationDbContext db;
        public CourseService()
        {
            db = new ApplicationDbContext();
        }
        public List<CourseViewModel> GetAllCourses()
        {
            var courses = db.Courses.ToList();
            var viewModel = new List<CourseViewModel>();
            foreach (var item in courses)
            {
                var temp = new CourseViewModel
                {
                    Name = item.Name,
                    ID = item.ID
                };
                viewModel.Add(temp);
            }
            return viewModel;
        }

        public List<AssignmentViewModel> GetAllUserAssignments(int userID)
        {
            var allCourses = GetCoursesWhereStudent(userID);
            List <AssignmentViewModel> allAssignments = new List<AssignmentViewModel>();
            AssignmentsService tmp = new AssignmentsService();
            foreach(var item in allCourses)
            {
                allAssignments.AddRange(tmp.GetAssignmentsInCourse(item.ID));
            }
            return allAssignments;
        }
        public CourseOverViewModel GetAllInfo(int userID)
        {
            CourseOverViewModel viewModel = new CourseOverViewModel
            {
                whereTeacher = GetCoursesWhereTeacher(userID),
                whereStudent = GetCoursesWhereStudent(userID),
                assignments = GetAllUserAssignments(userID)
            };
            return viewModel;

        }
        public List<Course> GetCoursesWhereStudent(int userID)
        {
            List<Course> result = (from c in db.CoursesStudents
                                  join cn in db.Courses on c.CourseID equals cn.ID
                                  join ct in db.Users on c.UserID equals ct.ID
                                  where (ct.ID == userID)
                                  select cn).ToList(); //SORRY I COULDNT DO IT :(
            return result;
        }
        
        public List<Course> GetCoursesWhereTeacher(int userID)
        {
            List<Course> result = (from c in db.CoursesTeachers
                                  join cn in db.Courses on c.CourseID equals cn.ID
                                  join ct in db.Users on c.TeacherID equals ct.ID
                                  where (ct.ID == userID)
                                  select cn).ToList();
            return result;
        }

        public List<User> GetAllStudents(int courseID)
        {
            List<User> result = (from c in db.CoursesStudents
                                   join cn in db.Courses on c.CourseID equals cn.ID
                                   join ct in db.Users on c.UserID equals ct.ID
                                   where (ct.ID == courseID)
                                   select ct).ToList();
            return result;
        }

        public List<User> GetAllTeachers(int courseID)
        {
            List<User> result = (from c in db.CoursesTeachers
                                   join cn in db.Courses on c.CourseID equals cn.ID
                                   join ct in db.Users on c.TeacherID equals ct.ID
                                   where (ct.ID == courseID)
                                   select ct).ToList();
            return result;
        }
        public CourseViewModel GetCoursesById(int courseID)
        {
            var course = db.Courses.SingleOrDefault(x => x.ID == courseID);
            if (course == null)
            {
                //TODO kasta
            }
            /*
            var milestones = db.Milestones.Where(x => x.AssignmendId == assignmentID).Select(x => new AssignmentMilestoneViewModel
            {
                Title = x.Title
            }).ToList();
                    public string Name { get; set; }
        public int ID { get; set; }
        public List<AssignmentViewModel> Asignments { get; set; }
        public List<User> Teachers { get; set; }
        public List<User> Students { get; set; }
        public List<Course> Courses { get; set; }
            */

            AssignmentsService tmp = new AssignmentsService();
            List<AssignmentViewModel> assignentViewModel = tmp.GetAssignmentsInCourse(courseID);
            var coursesAsTeacher = GetCoursesWhereTeacher(1);
            var coursesAsStudent = GetCoursesWhereStudent(1);
            var students = GetAllStudents(courseID);
            var teachers = GetAllTeachers(courseID);


            var viewModel = new CourseViewModel
            {
                Name = course.Name,
                ID = course.ID,
                Teachers = teachers,
                Students = students,
                CoursesAsStudent = coursesAsStudent,
                CoursesAsTeacher = coursesAsTeacher,
                Assignments = assignentViewModel
            };
            return viewModel;
        }
        
    }
}