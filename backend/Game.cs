using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography;

namespace server {
    public class Game {
        private readonly Client[] _clients;

        public Game() {
            Guid = Guid.NewGuid();
            _clients = new Client[] {null, null};
            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider()) {
                var data = new byte[8];
                rng.GetBytes(data);

                Password = Convert.ToBase64String(data);
            }

            Rows = 8;
            Cols = 8;
            VesselLengths = new List<int>(new[] {2, 3, 3, 5});
        }

        public Guid Guid { get; }
        public string Password { get; }

        public int Rows { get; }
        public int Cols { get; }
        public List<int> VesselLengths { get; }

        public ReadOnlyCollection<Client> GetClients() {
            return Array.AsReadOnly(_clients);
        }

        /// <summary>
        ///     Adds a new Client to the game
        /// </summary>
        /// <returns>A new Client or null if the game is already full</returns>
        public Client AddClient() {
            if (_clients[0] != null && _clients[1] != null) return null;

            var c = new Client(new PlayerBoard(Rows, Cols, VesselLengths));
            if (_clients[0] == null)
                _clients[0] = c;
            else
                _clients[1] = c;

            return c;
        }
    }
}