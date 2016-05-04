using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RipCore.Models.ViewModels
{
    public class AdminIndexViewModel
    {
        public List<PersonViewModel> Persons { get; set; }
        public List<CourseViewModel> Courses { get; set; }
    }
}