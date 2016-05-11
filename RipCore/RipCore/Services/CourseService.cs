using RipCore.Models;
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
                    ID = item.ID,
                    Year = item.Year,
                    Semester = item.Semester
                    
                };
                viewModel.Add(temp);
            }
            return viewModel;
        }

        public List<AssignmentViewModel> GetAllUserAssignments(string userID)
        {
            var allStudentCourses = GetCoursesWhereStudent(userID);
            var allTeacherCourses = GetCoursesWhereTeacher(userID);
            List <AssignmentViewModel> allAssignments = new List<AssignmentViewModel>();
            AssignmentsService tmp = new AssignmentsService();
            foreach(var item in allStudentCourses)
            {
                allAssignments.AddRange(tmp.GetAssignmentsInCourse(item.ID));
            }
            foreach (var item in allTeacherCourses)
            {
                var teacherAssignments = tmp.GetAssignmentsInCourse(item.ID);
                foreach (var entry in teacherAssignments)
                {
                    entry.IsTeacher = true;
                }
                allAssignments.AddRange(teacherAssignments);
            }
            return allAssignments;
        }
        public CourseOverViewModel GetAllInfo(string userID)
        {
            string strID = (from u in db.Users where u.Id == userID select u.Id).SingleOrDefault().ToString();
            CourseOverViewModel viewModel = new CourseOverViewModel
            {
                Name = (from u in db.Users where u.Id == userID select u.FullName).SingleOrDefault().ToString(),
                UserID = userID,
                whereTeacher = GetCoursesWhereTeacher(userID),
                whereStudent = GetCoursesWhereStudent(userID),
                assignments = GetAllUserAssignments(userID)
            };
            return viewModel;

        }
        public AdminCourseOverView GetCourseByID(int courseID)
        {
            var result = (from x in db.Courses
                          where x.ID == courseID
                          select x).SingleOrDefault();

            if (result == null)
            {
                //TODO kasta villu
                return null;
            }


            AdminCourseOverView viewModel = new AdminCourseOverView
            {
                ID = result.ID,
                Name = result.Name,
                Semester = result.Semester,
                Year = result.Year           
            };
            return viewModel;
        }
       
        public List<Course> GetCoursesWhereStudent(string userID)
        {
            List<Course> result = (from c in db.CoursesStudents
                                  join cn in db.Courses on c.CourseID equals cn.ID
                                  join ct in db.Users on c.UserID equals ct.Id
                                  where (ct.Id == userID)
                                  select cn).ToList(); 
            return result;
        }
        
        public List<Course> GetCoursesWhereTeacher(string userID)
        {
            List<Course> result = (from c in db.CoursesTeachers
                                  join cn in db.Courses on c.CourseID equals cn.ID
                                  join ct in db.Users on c.TeacherID equals ct.Id
                                  where (ct.Id == userID)
                                  select cn).ToList();
            return result;
        }

        public List<ApplicationUser> GetAllStudents(int courseID)
        {
            List<ApplicationUser> result = (from c in db.CoursesStudents
                                   join cn in db.Courses on c.CourseID equals cn.ID
                                   join ct in db.Users on c.UserID equals ct.Id
                                   where (cn.ID == courseID)
                                   select ct).ToList();
            return result;
        }

        public List<ApplicationUser> GetAllTeachers(int courseID)
        {
            List<ApplicationUser> result = (from c in db.CoursesTeachers
                                   join cn in db.Courses on c.CourseID equals cn.ID
                                   join ct in db.Users on c.TeacherID equals ct.Id
                                   where (cn.ID == courseID)
                                   select ct).ToList();
            return result;
        }
        public string getCourseNameByID(int courseID)
        {
            Course result = (from c in db.Courses
                          where c.ID == courseID
                          select c).SingleOrDefault();
            return (result.Name);
        }
        public CourseViewModel GetCoursesById(int courseID, string userID)
        {
            var course = db.Courses.SingleOrDefault(x => x.ID == courseID);
            if (course == null)
            {
                //TODO kastah
                int petur = 0;
                string strengur = "helaluja";
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
            var coursesAsTeacher = GetCoursesWhereTeacher(userID);
            var coursesAsStudent = GetCoursesWhereStudent(userID);
            var students = GetAllStudents(courseID);
            var teachers = GetAllTeachers(courseID);

            string userName = (from u in db.Users where u.Id == userID select u.FullName).SingleOrDefault().ToString();
            CourseViewModel viewModel = new CourseViewModel
            {
                Name = course.Name,
                ID = course.ID,
                UserID = userID,
                UserName = userName,
                Teachers = teachers,
                Students = students,
                Year = course.Year,
                Semester = course.Semester,
                CoursesAsStudent = coursesAsStudent,
                CoursesAsTeacher = coursesAsTeacher,
                Assignments = assignentViewModel,
                isTeacher = false,
            };
            return viewModel;
        }
        
    }
}