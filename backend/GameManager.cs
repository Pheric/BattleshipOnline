using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace server {
    public class GameManager {
        private static GameManager _instance;
        private readonly List<Game> _games;

        private GameManager() {
            _games = new List<Game>();
        }

        public bool AuthenticateUser(string gameGuid, IRequestCookieCollection cookies) {
            if (!Guid.TryParse(gameGuid, out var guid))
                return false;

            if (!cookies.ContainsKey("id") || !cookies.ContainsKey("secret"))
                return false;

            if (!Guid.TryParse(cookies["id"], out var clientGuid))
                return false;
            string clientSecret = cookies["secret"];

            return GameManager.GetInstance().AuthenticateUser(guid, clientGuid, clientSecret);
        }
        public bool AuthenticateUser(Guid gameGuid, Guid clientGuid, string clientSecret) {
            Game game = this.GetGameById(gameGuid);
            if (game == null)
                return false;

            Client c = this.GetClientById(clientGuid);
            return c != null && c.AuthCookie == clientSecret;
        }

        /// <summary>
        /// Registers a client with their game for the first time
        /// </summary>
        /// <returns>the new Client object or null if the game doesn't exist or is full</returns>
        public Client RegisterClient(Guid gameGuid, string gamePassword) {
            Game game = _games.Find(g => g.Guid == gameGuid && g.Password == gamePassword);

            return game?.AddClient();
        }

        /// <summary>
        /// Creates a new Game
        /// </summary>
        /// <returns>a new Game</returns>
        public Game CreateGame() {
            var g = new Game();
            _games.Add(g);

            return g;
        }

        /// <summary>
        /// Game has ended. Remove it.
        /// </summary>
        /// <param name="game">The Guid of the Game to remove</param>
        /// <returns>Success</returns>
        public bool CloseGame(Guid game) {
            var g = this.GetGameById(game);
            return g != null && _games.Remove(g);
        }

        public Game GetGameById(string id) => Guid.TryParse(id, out var guid) ? GetGameById(guid) : null;
        public Game GetGameById(Guid id) => _games.Find(g => g.Guid == id);

        public Client GetClientById(string id) => Guid.TryParse(id, out var guid) ? GetClientById(guid) : null;
        public Client GetClientById(Guid id) =>
            _games.SelectMany(g => g.GetClients()).FirstOrDefault(c => c != null && id == c.Id);

        public static GameManager GetInstance() => _instance ?? (_instance = new GameManager());
    }
}