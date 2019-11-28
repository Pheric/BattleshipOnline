using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using server;
using System.Net.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace api.Controllers {
    [Route("[controller]")]
    [ApiController]
    public class GameController : ControllerBase {
        /*
         * GET /game
         * Creates a new Game
         */
        [HttpGet]
        public Game Get() {
            return GameManager.Getinstance().CreateGame();
        }
        
        [HttpPost("{guid}/register")]
        public ActionResult<Client> Register(string guid, string password) {
            if (!Guid.TryParse(guid, out var gameGuid))
                return BadRequest();
            
            Client c = GameManager.Getinstance().RegisterClient(gameGuid, password);
            if (c == null)
                return NotFound();
            
            Response.Cookies.Append("id", c.Id.ToString());
            Response.Cookies.Append("secret", c.AuthCookie);

            return Ok(c);
        }

        [HttpGet("{guid}/poll")]
        public ActionResult<Game> Poll(string guid) {
            if (!Guid.TryParse(guid, out var gameGuid))
                return BadRequest();
            
            if (!Request.Cookies.ContainsKey("id") || !Request.Cookies.ContainsKey("secret"))
                return BadRequest();

            if (!Guid.TryParse(Request.Cookies["id"], out var clientGuid))
                return BadRequest();
            string clientSecret = Request.Cookies["secret"];

            if (!GameManager.Getinstance().AuthenticateUser(gameGuid, clientGuid, clientSecret))
                return Unauthorized();
            
            // TODO
            
            return Ok();
        }
    }
}