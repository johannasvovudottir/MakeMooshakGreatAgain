using RipCore.Models;
using RipCore.Models.ViewModels;
using RipCore.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft;
using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace RipCore.Services
{
 
    public class AccountsService
    {
        private ApplicationDbContext db;
        public AccountsService()
        {
            db = new ApplicationDbContext();
        }

        public bool GetIdByUser(string name, ref string userID)
        {
            try
            {
                userID = db.Users.First(u => u.UserName == name).Id;
            }
            catch
            {
                return false;
            }
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

        private SecurityState GetHighestUserPrivilege(string userID, int? courseID)
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


        public bool RequestJson(LoginViewModel login, ref string json)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(@"http://centris.dev.nem.ru.is/api/api/v1/login");
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string jsonconn = "{\"user\":\"" + login.Username + "\"," +
                              "\"pass\":\"" + login.Password + "\"}";

                streamWriter.Write(jsonconn);
                streamWriter.Flush();
                streamWriter.Close();
            }

            HttpWebResponse httpResponse = null;
            try
            {
                httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    json = streamReader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public CentrisViewModel GetCentrisUser(string json)
        {

            CentrisViewModel model = JsonConvert.DeserializeObject<CentrisViewModel>(json);
            return model;
        }
    }
}