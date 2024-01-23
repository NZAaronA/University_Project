using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using A2.Models;
using Microsoft.EntityFrameworkCore;

namespace A2.Data
{
    public class A2Repo : IA2Repo
    {
        private readonly A2DbContext _dbContext;

        public A2Repo(A2DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public User GetUserByUsername(string username)
        {
            User? user = _dbContext.Users.FirstOrDefault(e => e.UserName == username);
            return user;
        }

        public Organizer GetOrganizerByName(string name)
        {
            Organizer? organizer = _dbContext.Organizers.FirstOrDefault(e => e.Name == name);
            return organizer;
        }

        public void AddUser(User user)
        {
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
        }
        public bool ValidLogin(string username, string password)
        {
            User u = _dbContext.Users.FirstOrDefault(e => e.UserName == username && e.Password == password);
            if (u == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public Product GetProductById(int id)
        {
            Product? product = _dbContext.Products.FirstOrDefault(e => e.Id == id);
            return product;
        }

        public bool ValidOrganizer(string name, string password)
        {
            Organizer u = _dbContext.Organizers.FirstOrDefault(e => e.Name == name && e.Password == password);
            if (u == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void AddEvent(Event e)
        {
            _dbContext.Events.Add(e);
            _dbContext.SaveChanges();
        }

        public IEnumerable<Event> GetAllEvents()
        {
            IEnumerable<Event> allEvents = _dbContext.Events.ToList();
            return allEvents;
        }

        public Event GetEvent(int id)
        {
            return _dbContext.Events.FirstOrDefault(e => e.Id == id);
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}