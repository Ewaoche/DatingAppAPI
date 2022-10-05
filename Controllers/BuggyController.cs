using FinalDatingApp.Data;
using FinalDatingApp.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalDatingApp.Controllers
{
    public class BuggyController:ControllerBase
    {
        private readonly DataContext _context;
        public BuggyController(DataContext dataContext)
        {
            _context = dataContext;

        }

        [Authorize]
        [HttpPost("auth")]
        public ActionResult<string> GetSecret()
        {
            return "secret text";
        }

        [Authorize]
        [HttpPost("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("this is a bad request");
        }


        [Authorize]
        [HttpPost("not-found")]
        public ActionResult<AppUser> GetNotFound()
        {
            var thing = _context.Users.Find(-1);
            if (thing == null) return NotFound();
            return Ok(thing);
        }



        [Authorize]
        [HttpPost("server-error")]
        public ActionResult<string> GetServerError()
        {
            var thing = _context.Users.Find(-1);
            var thingToReturn = thing.ToString();
            return thingToReturn;
        }

    }
}
