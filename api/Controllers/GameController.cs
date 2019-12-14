using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using server;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
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
            return GameManager.GetInstance().CreateGame();
        }
        
        [HttpPost("{guid}/register")]
        public ActionResult<Client> Register(string guid, string password) {
            if (!Guid.TryParse(guid, out var gameGuid))
                return BadRequest();
            
            Client c = GameManager.GetInstance().RegisterClient(gameGuid, password);
            if (c == null)
                return NotFound();
            
            Response.Cookies.Append("id", c.Id.ToString());
            Response.Cookies.Append("secret", c.AuthCookie);

            return Ok(c);
        }

        [HttpPost("{guid}/setup")]
        public ActionResult Setup(string guid, SetupRequest setupRequest) {
            if (!GameManager.GetInstance().AuthenticateUser(guid, Request.Cookies))
                return NotFound();

            var g = GameManager.GetInstance().GetGameById(guid);
            if (g.State != GameState.SETUP)
                return BadRequest();
            
            var c = GameManager.GetInstance().GetClientById(Request.Cookies["id"]);
            if (c.Board.IsSet())
                return Conflict();

            try {
                c.Board.Vessels = setupRequest.Vessels;
                g.IncrementState();
            } catch (ArgumentException e) {
                return BadRequest(e.Message);
            }

            return NoContent();
        }

        [HttpPost("{guid}/strike")]
        public ActionResult<bool> Strike(string guid, StrikeRequest strikeRequest) {
            if (!GameManager.GetInstance().AuthenticateUser(guid, Request.Cookies))
                return NotFound();
            
            var g = GameManager.GetInstance().GetGameById(guid);
            if (g.State != GameState.PLAYER1 && g.State != GameState.PLAYER2)
                return BadRequest();
            
            var c = GameManager.GetInstance().GetClientById(Request.Cookies["id"]);
            if (c != g.GetActiveClient())
                return BadRequest();

            
            
            var opponent = g.GetClients()[0] == c ? g.GetClients()[1] : g.GetClients()[0];

            try {
                var ret = opponent.Board.StrikeCell(strikeRequest.Coordinate);
                g.IncrementState();
                return Ok(ret);
            } catch (ArgumentException e) {
                return BadRequest();
            }
        }

        [HttpGet("{guid}/poll")]
        public ActionResult<PollResponse> Poll(string guid) {
            if (!GameManager.GetInstance().AuthenticateUser(guid, Request.Cookies))
                return NotFound();

            // guaranteed non-null because auth passed
            var g = GameManager.GetInstance().GetGameById(guid);
            
            var response =
                new PollResponse(g.State, g.GetActiveClient()?.Board?.StrikeRecord);

            return Ok(response);
        }
    }
}