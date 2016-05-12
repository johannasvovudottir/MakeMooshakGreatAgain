using RipCore.Models.Entities;
using System.Data.Entity;
using RipCore.Models;

namespace RipCore.Tests
{
    class MockDataContext : IAppDataContext
    {

        public IDbSet<ApplicationUser> Users { get; set; }
        public IDbSet<Assignment> Assignments { get; set; }
        public IDbSet<Milestone> Milestones { get; set; }
        public IDbSet<Course_Teacher> CoursesTeachers { get; set; }
        public IDbSet<Course_Student> CoursesStudents { get; set; }
        public IDbSet<Solution> Solutions { get; set; }
        public IDbSet<Submission> Submission { get; set; }
        public IDbSet<Course> Courses { get; set; }
        public IDbSet<Admin> Admins { get; set; }


        /// <summary>
        /// Sets up the fake database.
        /// </summary>
        public MockDataContext()
        {
            Users = new InMemoryDbSet<ApplicationUser>();
            Assignments = new InMemoryDbSet<Assignment>();
            Milestones = new InMemoryDbSet<Milestone>();
            CoursesTeachers = new InMemoryDbSet<Course_Teacher>();
            CoursesStudents = new InMemoryDbSet<Course_Student>();
            Solutions = new InMemoryDbSet<Solution>();
            Submission = new InMemoryDbSet<Submission>();
            Courses = new InMemoryDbSet<Course>();
            Admins = new InMemoryDbSet<Admin>();


            // We're setting our DbSets to be InMemoryDbSets rather than using SQL Server.

        }



        // TODO: bætið við fleiri færslum hér
        // eftir því sem þeim fjölgar í AppDataContext klasanum ykkar!

        public int SaveChanges()
        {
            // Pretend that each entity gets a database id when we hit save.
            int changes = 0;

            return changes;
        }

        public void Dispose()
        {
            // Do nothing!
        }
    }
}
