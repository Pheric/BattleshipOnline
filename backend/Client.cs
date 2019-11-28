using System;
using System.Security.Cryptography;

namespace server {
    public class Client {
        public Guid Id { get; }
        public string AuthCookie { get; }
        public PlayerBoard Board;

        public Client(PlayerBoard board) {
            Id = Guid.NewGuid();
            Board = board;

            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider()) {
                var data = new byte[64];
                rng.GetBytes(data);

                AuthCookie = Utils.SanitizeString(Convert.ToBase64String(data));
            }
        }
    }
}