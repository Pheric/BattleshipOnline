using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography;

namespace server {
    public class Game {
        public Guid Guid { get; }
        public string Password { get; }

        private GameState _state;
        public GameState State => _state;

        public int Rows { get; }
        public int Cols { get; }
        public List<int> VesselLengths { get; }
        
        private readonly Client[] _clients;
        public ReadOnlyCollection<Client> GetClients() {
            return Array.AsReadOnly(_clients);
        }

        public Game() {
            Guid = Guid.NewGuid();
            _clients = new Client[] {null, null};
            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider()) {
                var data = new byte[9];
                rng.GetBytes(data);

                Password = Utils.SanitizeString(Convert.ToBase64String(data).Remove(8));
            }
            _state = GameState.REGISTRATION;

            Rows = 8;
            Cols = 8;
            VesselLengths = new List<int>(new[] {2, 3, 3, 5});
        }

        /// <summary>
        ///     Adds a new Client to the game and updates the game's state automatically
        /// </summary>
        /// <returns>A new Client or null if the game is already full</returns>
        public Client AddClient() {
            if (_clients[0] != null && _clients[1] != null) return null;

            var c = new Client(new PlayerBoard(Rows, Cols, VesselLengths));
            if (_clients[0] == null) {
                _clients[0] = c;
            } else {
                _clients[1] = c;
                this._state = GameState.SETUP;
            }

            return c;
        }

        public Client GetActiveClient() => State == GameState.PLAYER1 || State == GameState.REGISTRATION ? _clients[0] : State == GameState.PLAYER2 ? _clients[1] : null;
    }
}