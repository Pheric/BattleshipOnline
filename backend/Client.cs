using System;
using System.Security.Cryptography;

namespace server {
    public class Client {
        public PlayerBoard Board;

        public Client(PlayerBoard board) {
            Id = Guid.NewGuid();
            Board = board;

            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider()) {
                var data = new byte[64];
                rng.GetBytes(data);

                AuthCookie = Convert.ToBase64String(data);
            }
        }

        public Guid Id { get; }
        public string AuthCookie { get; }
    }
}