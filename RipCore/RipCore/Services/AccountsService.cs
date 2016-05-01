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

        public AccountsService()
        {
            db = new ApplicationDbContext();
        }

        public bool IsValid(string username, string password)
        {
            using (var db = new ApplicationDbContext())
            {
                return db.Users.Any(u => u.UserName == username
                    && u.Password == password);
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