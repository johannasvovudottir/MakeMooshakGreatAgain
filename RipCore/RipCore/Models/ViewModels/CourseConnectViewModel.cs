using RipCore.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RipCore.Models.ViewModels
{
    public class CourseConnectViewModel
    {
        public Course CurrentCourse{ get; set;}
        public List<PersonViewModel> UnConnectedUsers {get; set;} 
        public List<PersonViewModel> Teachers { get; set; }
        public List<PersonViewModel> Students { get; set; }

    }
}