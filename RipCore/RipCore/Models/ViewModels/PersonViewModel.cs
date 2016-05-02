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
        public int ID { get; set; }
        public string FullName { get; set; }
        public int Ssn { get; set; }

    }
}