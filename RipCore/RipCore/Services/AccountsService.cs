using RipCore.Models;
using RipCore.Models.ViewModels;
using RipCore.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace RipCore.Services
{
    public class AccountsService
    {
        private ApplicationDbContext db;
        private EncryptionService encService;
        public AccountsService()
        {
            db = new ApplicationDbContext();
            encService = new EncryptionService();
        }

        public bool GetIdByUser(string name, ref string userID)
        {
            string id = db.Users.First(u => u.UserName == name).Id;
            if (id == null)
            {
                return false;
            }
            userID = id;
            return true;
        }

        public bool IsUserQualified(string role, string userID, int courseID)
        {
            using (var db = new ApplicationDbContext())
            {
                if (role.ToLower() == "teacher")
                {
                    return db.CoursesTeachers.Any(u => u.TeacherID == userID
                        && u.CourseID == courseID);
                }
                else
                {
                    return db.CoursesStudents.Any(u => u.UserID == userID
                        && u.CourseID == courseID);
                }
            }
        }



        /*
        public List<string> GetRoles(int userID)
        {
            List<string> result = (from c in db.CoursesStudents
                                   join cn in db.Courses on c.CourseID equals cn.ID
                                   join ct in db.Users on c.UserID equals ct.ID
                                   where (ct.ID == userID)
                                   select cn.Name).ToList();

            return result;
        }
        */
    }
}