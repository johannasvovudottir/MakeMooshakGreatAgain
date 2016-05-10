using RipCore.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RipCore.Models.ViewModels
{
    public class AdminCourseOverView
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Áfangi verður að hafa nafn")]
        
        public string Name { get; set; }
        [Required(ErrorMessage = "Áfangi verður að hafa kennsluár")]
        //[StringLength(4,ErrorMessage = "The year must have four digits",MinimumLength =4)]
        [Range(2000,2100, ErrorMessage = "Ártalið verður að vera á milli 2000 og 2050")]
        public int Year { get; set; }
        [Required(ErrorMessage = "Áfangi verður að hafa haust eða vorönn")]
        public string Semester { get; set; }
        public Course toCourse() {
            Course courseToReturn = new Course {
                                    ID = this.ID,
                                    Name = this.Name,
                                    Year = this.Year,
                                    Semester = this.Semester
                                    };
            return courseToReturn;
        }

    }
}