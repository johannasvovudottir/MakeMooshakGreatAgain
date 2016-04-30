using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RipCore.Models.Entities
{
    public class Student
    {
        [Key]
        public int ID { get; set; }
        public int Year { get; set; }
        public int Degree { get; set; }
        public int Field { get; set; }
    }
}