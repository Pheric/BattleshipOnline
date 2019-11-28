using NUnit.Framework;
using server;

namespace serverTest {
    [TestFixture]
    public class GameTest {
        [Test]
        public void GameCreationTest() {
            GameManager gm = new GameManager();
            Assert.IsNull(gm.GetGameByAuth(""));

            Game g = gm.CreateGame();
            Assert.IsNotNull(g);
            
            AddClientsTest(g);
            Assert.AreEqual(g.GetClients().Count, 2);
        }
        
        void AddClientsTest(Game g) {
            var c1 = g.AddClient();
            Assert.IsNotNull(c1);
            Assert.IsNotEmpty(c1.AuthCookie);
            
            var c2 = g.AddClient();
            Assert.IsNotNull(c2);
            Assert.IsNotEmpty(c2.AuthCookie);
            
            var invalidByClientCount = g.AddClient();
            Assert.IsNull(invalidByClientCount);
        }
    }
}