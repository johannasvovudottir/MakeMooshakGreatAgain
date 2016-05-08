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

        public SecurityRedirect VerifySecurityLevel(bool auth, SecurityState secLevel, string userID, int? courseID = null)
        {
            string[] roles = { "User", "Student", "Teacher", "Admin" };
            SecurityRedirect redirect = new SecurityRedirect { Redirect = true };
            //If user isnt logged in...
            if (!auth)
            {
                redirect.ActionName = "Login";
                redirect.ControllerName = "Account";
                return redirect;
            }

            //Make sure the highest security level the user has is sufficient for the minimum security level.
            if(!(secLevel <= GetHighestUserPrivilege(userID, courseID)))
            {
                redirect.ActionName = "Index";
                redirect.ControllerName = "User";
                return redirect;
            }
            
            //If it reaches this part of the code, it means we have no reason to deny entry.
            redirect.Redirect = false;
            return redirect;
        }

        public SecurityState GetHighestUserPrivilege(string userID, int? courseID)
        {
            int securityLevel = 0;
            using (var db = new ApplicationDbContext())
            {
                if (db.Admins.Any(a => a.UserID == userID))
                {
                    int secId = Convert.ToInt32(SecurityState.ADMIN);
                    if (securityLevel <= secId)
                    {
                        securityLevel = secId;
                    }
                }

                if (courseID.HasValue)
                {
                    if (db.CoursesTeachers.Any(u => u.TeacherID == userID && u.CourseID == courseID))
                    {

                        int secId = Convert.ToInt32(SecurityState.TEACHER);
                        if (securityLevel <= secId)
                        {
                            securityLevel = secId;
                        }
                    }

                    if (db.CoursesStudents.Any(x => x.UserID == userID && x.CourseID == courseID))
                    {
                        int secId = Convert.ToInt32(SecurityState.STUDENT);
                        if (securityLevel <= secId)
                        {
                            securityLevel = secId;
                        }
                    }
                }
            }
            SecurityState state = (SecurityState)Enum.ToObject(typeof(SecurityState), securityLevel);
            return state;
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
                else if(role.ToLower() == "student")
                {
                    return db.CoursesStudents.Any(u => u.UserID == userID
                        && u.CourseID == courseID);
                }
                else if(role.ToLower() == "admin")
                {
                    return true;
                }
                else //In case of anonymous, or user that doesnt have any privileges
                {
                    
                    return false;
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