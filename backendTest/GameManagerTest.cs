using System;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using server;

namespace serverTest {
    [TestFixture]
    public class GameManagerTest {
        private Game _game;

        [Test]
        public void CreateGame() {
            Assert.IsNotNull(GameManager.Getinstance());

            _game = GameManager.Getinstance().CreateGame();
            Assert.IsNotNull(_game);
        }
        
        [Test]
        public void RegisterClientsTest() {
            var invalidByInvalidGuid = GameManager.Getinstance().RegisterClient(Guid.NewGuid(), _game.Password);
            Assert.IsNull(invalidByInvalidGuid);

            var invalidByInvalidPassword = GameManager.Getinstance().RegisterClient(_game.Guid, "");
            Assert.IsNull(invalidByInvalidPassword);
            
            var c1 = GameManager.Getinstance().RegisterClient(_game.Guid, _game.Password);
            Assert.IsNotNull(c1);
            var c2 = GameManager.Getinstance().RegisterClient(_game.Guid, _game.Password);
            Assert.IsNotNull(c2);

            var invalidByClientCount = GameManager.Getinstance().RegisterClient(_game.Guid, _game.Password);
            Assert.IsNull(invalidByClientCount);
        }
    }
}