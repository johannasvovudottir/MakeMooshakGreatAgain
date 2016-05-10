using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RipCore.Models.Entities;

namespace RipCore.Models.ViewModels
{
    public class PersonViewModel
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string ID { get; set; }
        public int Ssn { get; set; }
        public bool isChecked { get; set; }
        public bool CentrisUser { get; set; }
        public string Role { get; set; }
    }
}