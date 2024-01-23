using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.Unicode;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Xml.Linq;
using A2.Models;
using A2.Data;
using A2.Dto;
using System.Security.Claims;
using System.Collections;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace A2.Controllers
{
    [Route("webapi")]
    [ApiController]
    public class A2Controller : Controller
    {
        private readonly IA2Repo _repository;

        public A2Controller(IA2Repo repository)
        {
            _repository = repository;
        }

        //POST /webapi/Register/
        [HttpPost("Register")]
        public ActionResult<User> RegisterUser(User user)
        {
            User u = _repository.GetUserByUsername(user.UserName);
            Organizer o = _repository.GetOrganizerByName(user.UserName);
            if (u == null && o == null)
            {
                _repository.AddUser(user);
                return Ok("User successfully registered.");
            }
            else
            {
                return Ok(String.Format("UserName {0} is not available.", u.UserName));
            }
        }

        //GET /webapi/PurchaseItem/{id}
        [Authorize(AuthenticationSchemes = "Authentication")]
        [Authorize(Policy = "RegisteredUserOnly")]
        [HttpGet("PurchaseItem/{id}")]
        public ActionResult<PurchaseOutput> PurchaseItem(int id)
        {
            Product product = _repository.GetProductById(id);
            if (product == null)
            {
                return BadRequest(String.Format("Product {0} not found", id));
            }
            else
            {
                ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
                Claim c = ci.FindFirst("normalUser");
                string username = c.Value;
                PurchaseOutput productOut = new PurchaseOutput
                {
                    userName = username,
                    productId = product.Id
                };
                return Ok(productOut);
            }
        }

        //POST /webapi/AddEvent
        [Authorize(AuthenticationSchemes = "Authentication")]
        [Authorize(Policy = "OrganizerOnly")]
        [HttpPost("AddEvent")]
        public ActionResult AddEvent(EventInput eventInput)
        {
            DateTime dateStart, dateEnd;
            bool startCorrect = DateTime.TryParseExact(eventInput.start, "yyyyMMddTHHmmssZ", null, DateTimeStyles.None, out dateStart);
            bool endCorrect = DateTime.TryParseExact(eventInput.end, "yyyyMMddTHHmmssZ", null, DateTimeStyles.None, out dateEnd);

            if (startCorrect == false && endCorrect == false)
            {
                return BadRequest("The format of Start and End should be yyyyMMddTHHmmssZ.");
            }
            else if (startCorrect == false)
            {
                return BadRequest("The format of Start should be yyyyMMddTHHmmssZ.");
            }
            else if (endCorrect == false)
            {
                return BadRequest("The format of End should be yyyyMMddTHHmmssZ.");
            }
            else
            {
                Event e = new Event { Start = eventInput.start, End = eventInput.end, Summary = eventInput.summary, Description = eventInput.description, Location = eventInput.location };
                _repository.AddEvent(e);
                return Ok("Success");
            }
        }
        //GET /webapi/EventCount
        [Authorize(AuthenticationSchemes = "Authentication")]
        [Authorize(Policy = "RegisteredUsersAndOrganizers")]
        [HttpGet("EventCount")]
        public int GetEventCount()
        {
            IEnumerable<Event> allEvents = _repository.GetAllEvents();
            return allEvents.Count();
        }

        //GET /webapi/Event/{id}
        [Authorize(AuthenticationSchemes = "Authentication")]
        [Authorize(Policy = "RegisteredUsersAndOrganizers")]
        [HttpGet("Event/{id}")]
        public ActionResult GetCalendar(int id)
        {
            Event e = _repository.GetEvent(id);
            if (e == null)
            {
                return BadRequest(String.Format("Event {0} does not exist.", id));
            }
            else
            {
                Response.Headers.Add("Content-Type", "text/calendar");
                return Ok(e);
            }
        }

    }
}