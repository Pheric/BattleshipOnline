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
        public ActionResult<PollResponse> Poll(string guid) {
            Game g = GameManager.Getinstance().AuthenticateUser(guid, Request.Cookies);
            if (g == null)
                return NotFound();
            
            var response =
                new PollResponse(g.Guid, g.State, g.VesselLengths, g.GetActiveClient()?.Board?.ExportVessels());

            return Ok(response);
        }
    }
}