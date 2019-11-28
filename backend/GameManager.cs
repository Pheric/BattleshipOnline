using System.Collections.Generic;

namespace server {
    public class GameManager {
        private readonly List<Game> _games;

        public GameManager() {
            _games = new List<Game>();
        }

        /// <summary>
        ///     Gets a Game instance given a clients Auth code
        /// </summary>
        /// <param name="clientAuthCookie">secret found in the client's auth cookie</param>
        /// <returns>the client's Game or null if not found</returns>
        public Game GetGameByAuth(string clientAuthCookie) {
            foreach (var g in _games)
            foreach (var client in g.GetClients())
                if (client.AuthCookie.Equals(clientAuthCookie))
                    return g;

            return null;
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
    }
}