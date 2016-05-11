using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RipCore.Models.Entities
{
    public class AppUser
    {
        public int ID { get; set; }
        // public int Year { get; set; }
        // public int Degree { get; set; }
        // public int Field { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Ssn { get; set; }
        // public School schoolName { get; set; }
        //public List<Course> courses; //{ get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Passkey { get; set; }
    }
}