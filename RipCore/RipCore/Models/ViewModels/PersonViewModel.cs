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
        public string Password { get; set; }
        public string Passkey { get; set; }
        public string Email { get; set; }
        public int ID { get; set; }
        public int Ssn { get; set; }
    }
}