using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RipCore.Models.Entities
{
    public class Milestone
    {
        public int ID { get; set; }
        public int AssignmentID { get; set; }
        public int Weight { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public string Code { get; set; }
    }
}