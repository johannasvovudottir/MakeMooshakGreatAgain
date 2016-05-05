using RipCore.Models;
using RipCore.Models.Entities;
using RipCore.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RipCore.Services
{
    public class PersonService
    {
        private ApplicationDbContext db;

        public PersonService()
        {
            db = new ApplicationDbContext();
        }

        public List<PersonViewModel> GetAllPersons()
        {
            var Persons = db.Users.ToList();
            var viewModel = new List<PersonViewModel>();
            foreach (var item in Persons)
            {
                var temp = new PersonViewModel
                {
                    Name = item.FullName,
                    ID = item.Id
                };
                viewModel.Add(temp);
            }
            return viewModel;
        }
        public List<PersonViewModel> GetAllTeachers(int courseID)
        {
            CourseService query = new CourseService();
            List<ApplicationUser> teachers = query.GetAllTeachers(courseID);
            List<PersonViewModel> teachersToReturn = new List<PersonViewModel>();
            foreach (ApplicationUser person  in teachers)
            {


            }

            return null;
        }
        public PersonViewModel GetPersonById(string PersonID)
        {
            var result = (from x in db.Users
                          where x.Id == PersonID
                          select x).SingleOrDefault();

            if (result == null)
            {
                //TODO kasta villu
                return null;
            }

            var viewModel = new PersonViewModel
            {
                Name = result.FullName,
                ID = result.Id,
                Ssn = result.Ssn,
                PasswordHash = result.PasswordHash,
                Email = result.Email,
                Username = result.UserName
            };

            return viewModel;
        }
    }
}