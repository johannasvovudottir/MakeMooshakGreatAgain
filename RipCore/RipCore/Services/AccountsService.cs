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

        public bool GetIdByUser(string name, ref int userID)
        {
            int id = db.Users.First(u => u.UserName == name).ID;
            if (id == 0)
            {
                return false;
            }
            userID = id;
            return true;
        }

        public bool IsUserQualified(string role, int userID, int courseID)
        {
            using (var db = new ApplicationDbContext())
            {
                if (role.ToLower() == "Teacher")
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

        public bool IsValid(string username, string password)
        {
            User user = new User();
            using (var db = new ApplicationDbContext())
            {

                if(db.Users.Any(u => u.UserName == username))
                {
                    user = db.Users.First(u => u.UserName == username);
                }
                else
                {
                    return false;
                }

                string userdecrypt = encService.Decrypt(user.Password, user.Passkey);
                if(password == userdecrypt)
                {
                    return true;
                }
                return false;
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