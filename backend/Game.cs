using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace server {
    public class Game {
        public Guid Guid { get; }
        public string Password { get; }

        private GameState _state;
        public GameState State => _state;
        private GameState _prevState;

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
            _state = GameState.SETUP;
            _prevState = _state;

            Rows = 8;
            Cols = 8;
            VesselLengths = new List<int>(new[] {5, 4, 3, 3, 2});
        }

        /// <summary>
        /// Increments the Game's GameState intelligently, based on the current state.
        /// The API will call this to increment the state past SETUP (successful if both
        /// clients have set up their PlayerBoard) and to change firing roles (the player
        /// that may fire next).
        ///
        /// The TIMEOUT state must be set in a TODO timer loop
        /// but if this function is called while in this state, it will leave the TIMEOUT
        /// state.
        ///
        /// The VICTOR states will also automatically be set by this function when a move
        /// wins the game.
        ///
        /// If a VICTOR state is set, this method will close and delete this game instance
        /// through the GameManager.
        /// </summary>
        /// <returns>The new (current) GameState</returns>
        public GameState IncrementState() {
            switch (State) {
                case GameState.SETUP:
                    if (_clients.All(c => c != null && c.Board.IsSet())) {
                        this._state = new Random().Next(2) == 0 ? GameState.PLAYER1 : GameState.PLAYER2;
                        this._prevState = _state;
                    }
                    break;
                case GameState.PLAYER1:
                case GameState.PLAYER2:
                    if (this.IsComplete())
                        break;
                    this._state = this._state == GameState.PLAYER1 ? GameState.PLAYER2 : GameState.PLAYER1;
                    this._prevState = _state;
                    break;
                case GameState.PLAYER1VICTOR:
                case GameState.PLAYER2VICTOR:
                    GameManager.GetInstance().CloseGame(this.Guid);
                    break;
                default:
                    throw new ArgumentException("Invalid GameState on Game#IncrementState()");
            }

            return _state;
        }

        /// <summary>
        /// Adds a new Client to the game and updates the game's state automatically
        /// </summary>
        /// <returns>A new Client or null if the game is already full</returns>
        public Client AddClient() {
            if (_clients[0] != null && _clients[1] != null) return null;

            var c = new Client(new PlayerBoard(Rows, Cols, VesselLengths));
            if (_clients[0] == null) {
                _clients[0] = c;
            } else {
                _clients[1] = c;
            }

            return c;
        }

        /// <summary>
        /// Gets the active Client (if the Game is in a normal playing mode) or null
        /// </summary>
        /// <returns>The active Client or null</returns>
        public Client GetActiveClient() => State == GameState.PLAYER1 ? _clients[0] : State == GameState.PLAYER2 ? _clients[1] : null;

        /// <summary>
        /// Checks whether a player has won the game (based on ships remaining).
        /// Automatically sets the VICTOR state if so.
        /// </summary>
        /// <returns>Whether the game is over</returns>
        public bool IsComplete() {
            if (_state == GameState.PLAYER1VICTOR || _state == GameState.PLAYER2VICTOR)
                return true;

            if (_clients[0].Board.IsLost()) {
                _state = GameState.PLAYER2VICTOR;
                return true;
            }
            
            if (_clients[1].Board.IsLost()) {
                _state = GameState.PLAYER1VICTOR;
                return true;
            }

            return false;
        }
    }
}