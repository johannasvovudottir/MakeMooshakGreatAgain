using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RipCore.Models.Entities;

namespace RipCore.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Ssn { get; set; }
        public bool CentrisUser { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public interface IAppDataContext
    {
        IDbSet<ApplicationUser> Users { get; set; }
        IDbSet<Assignment> Assignments { get; set; }
        IDbSet<Milestone> Milestones { get; set; }
        IDbSet<Course_Teacher> CoursesTeachers { get; set; }
        IDbSet<Course_Student> CoursesStudents { get; set; }
        IDbSet<Solution> Solutions { get; set; }
        IDbSet<Submission> Submission { get; set; }
        IDbSet<Course> Courses { get; set; }
        IDbSet<Admin> Admins { get; set; }
        int SaveChanges();
    }


    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IAppDataContext
    {
        public IDbSet<Assignment> Assignments { get; set; }
        public IDbSet<Milestone> Milestones { get; set; }
        public IDbSet<Course_Teacher> CoursesTeachers { get; set; }
        public IDbSet<Course_Student> CoursesStudents { get; set; }
        // public DbSet<School> Schools { get; set; }
        public IDbSet<Solution> Solutions { get; set; }
        public IDbSet<Submission> Submission { get; set; }
        public IDbSet<Course> Courses { get; set; }
        public IDbSet<Admin> Admins { get; set; }



        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}





















/*
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RipCore.Models.Entities;

namespace RipCore.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Ssn { get; set; }
        public bool CentrisUser { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Milestone> Milestones { get; set; }
        public DbSet<Course_Teacher> CoursesTeachers { get; set; }
        public DbSet<Course_Student> CoursesStudents { get; set; }
       // public DbSet<School> Schools { get; set; }
        public DbSet<Solution> Solutions { get; set; }
        public DbSet<Submission> Submission { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Admin> Admins { get; set; }
       


        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}*/
