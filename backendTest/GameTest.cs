using System;
using NUnit.Framework;
using server;

namespace serverTest {
    [TestFixture]
    public class GameTest {
        [Test, Order(1)]
        public void GameCreationTest() {
            var g = new Game();
            Assert.IsNotNull(g);
            Assert.IsNotEmpty(g.Password);
            Assert.IsNotEmpty(g.VesselLengths);
        }

        [Test, Order(10)]
        public void ClientAdditionTest() {
            var g = new Game();
            
            var c1 = g.AddClient();
            Assert.IsNotNull(c1);
            Assert.IsNotEmpty(c1.AuthCookie);
            Assert.IsNotNull(c1.Board);
            Assert.IsNotNull(g.GetClients()[0]);
            Assert.IsNull(g.GetClients()[1]);
            
            var c2 = g.AddClient();
            Assert.IsNotNull(c2);
            Assert.IsNotEmpty(c2.AuthCookie);
            Assert.IsNotNull(c2.Board);
            Assert.IsNotNull(g.GetClients()[0]);
            Assert.IsNotNull(g.GetClients()[1]);

            var invalidByClientCount = g.AddClient();
            Assert.IsNull(invalidByClientCount);
            Assert.AreEqual(2, g.GetClients().Count);
        }
    }
}