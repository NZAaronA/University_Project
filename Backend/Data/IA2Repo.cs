using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using A2.Models;

namespace A2.Data
{
    public interface IA2Repo
    {
        public User GetUserByUsername(string username);
        public Organizer GetOrganizerByName(string name);
        public void AddUser(User user);
        public bool ValidLogin(string username, string password);
        public Product GetProductById(int id);
        public bool ValidOrganizer(string name, string password);
        public void AddEvent(Event e);
        public IEnumerable<Event> GetAllEvents();
        public Event GetEvent(int id);
    }
}