using System;
using System.Collections.Generic;

namespace server {
    public class GameManager {
        private static GameManager _instance;
        private readonly List<Game> _games;

        public GameManager() {
            _games = new List<Game>();
        }

        public bool AuthenticateUser(Guid gameGuid, Guid clientGuid, string clientSecret) {
            Console.WriteLine($"gameGuid: {gameGuid} clientGuid: {clientGuid} clientSecret: {clientSecret}");
            
            Game game = _games.Find(g => g.Guid == gameGuid);
            if (game == null)
                return false;

            foreach (var c in game.GetClients()) {
                if (c.Id == clientGuid && c.AuthCookie == clientSecret)
                    return true;
            }

            return false;
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
        ///     Creates a new Game
        /// </summary>
        /// <returns>a new Game</returns>
        public Game CreateGame() {
            var g = new Game();
            _games.Add(g);

            return g;
        }

        public static GameManager Getinstance() {
            if (_instance == null)
                _instance = new GameManager();

            return _instance;
        }
    }
}