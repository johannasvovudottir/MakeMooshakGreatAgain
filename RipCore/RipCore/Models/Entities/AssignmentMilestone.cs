using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RipCore.Models.Entities
{
    public class AssignmentMilestone
    {
        public int ID { get; set; }
        public int AssignmendId { get; set; }
        public string Title { get; set; }
    }
}