﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RipCore.Models.Entities
{
    public class Person
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public int Ssn { get; set; }
        // public School schoolName { get; set; }
        //public List<Course> courses; //{ get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}