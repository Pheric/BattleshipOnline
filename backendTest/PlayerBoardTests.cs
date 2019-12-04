using System;
using System.Collections.Generic;
using NUnit.Framework;
using server;

namespace serverTest {
    [TestFixture]
    public class PlayerBoardTests {
        [Test]
        public void Test() {
            PlayerBoard board = CreateBoard();
            
            TestVesselPopulation(board);
        }

        PlayerBoard CreateBoard() {
            var lens = new List<int>(new[] {3, 3, 5});
            PlayerBoard board = new PlayerBoard(10, 10, lens);

            Assert.AreEqual(10, board.Cols);
            Assert.AreEqual(10, board.Rows);
            Assert.AreEqual(board.VesselLengths, lens);

            return board;
        }
        
        void TestVesselPopulation(PlayerBoard board) {
            var valid = new List<Vessel>(new [] {
                new Vessel(new KeyValuePair<int, int>(7, 4), 3, VesselOrientation.Up),
                new Vessel(new KeyValuePair<int, int>(5, 3), 3, VesselOrientation.Left), 
                new Vessel(new KeyValuePair<int, int>(2, 2), 5, VesselOrientation.Right), 
            });
            Assert.DoesNotThrow(() => board.Vessels = valid);
            
            var invalidByBoundChecks = new List<Vessel>(new [] {
                new Vessel(new KeyValuePair<int, int>(7, 4), 3, VesselOrientation.Up),
                new Vessel(new KeyValuePair<int, int>(10, 3), 3, VesselOrientation.Left), 
                new Vessel(new KeyValuePair<int, int>(2, 2), 5, VesselOrientation.Right), 
            });
            Assert.Throws<ArgumentException>(() => board.Vessels = invalidByBoundChecks);
            
            var invalidByCount = new List<Vessel>(new [] {
                new Vessel(new KeyValuePair<int, int>(7, 4), 3, VesselOrientation.Up),
                new Vessel(new KeyValuePair<int, int>(8, 3), 3, VesselOrientation.Left),
            });
            Assert.Throws<ArgumentException>(() => board.Vessels = invalidByCount);
            
            var invalidByShipLengths = new List<Vessel>(new [] {
                new Vessel(new KeyValuePair<int, int>(7, 4), 3, VesselOrientation.Up),
                new Vessel(new KeyValuePair<int, int>(8, 3), 1, VesselOrientation.Left), 
                new Vessel(new KeyValuePair<int, int>(2, 2), 5, VesselOrientation.Right), 
            });
            Assert.Throws<ArgumentException>(() => board.Vessels = invalidByShipLengths);
            
            var invalidByCollision = new List<Vessel>(new [] {
                new Vessel(new KeyValuePair<int, int>(7, 4), 3, VesselOrientation.Up),
                new Vessel(new KeyValuePair<int, int>(5, 3), 3, VesselOrientation.Left), 
                new Vessel(new KeyValuePair<int, int>(2, 3), 5, VesselOrientation.Right), 
            });
            Assert.Throws<ArgumentException>((() => board.Vessels = invalidByCollision));
        }
        
        [Test]
        public void TestVesselBoundsCheck() {
            var valid = new Vessel(new KeyValuePair<int, int>(2, 4), 5, VesselOrientation.Down);
            Assert.AreEqual(true, valid.AssertPositionInBounds(10, 10));
            
            var valid2 = new Vessel(new KeyValuePair<int, int>(0, 9), 1, VesselOrientation.Up);
            Assert.AreEqual(true, valid2.AssertPositionInBounds(10, 10));
            
            var invalid = new Vessel(new KeyValuePair<int, int>(7, 1), 4, VesselOrientation.Right);
            Assert.AreEqual(false, invalid.AssertPositionInBounds(10, 10));
        }

        [Test]
        public void TestVesselCellRemoval() {
            var v = new Vessel(new KeyValuePair<int, int>(2, 1), 7, VesselOrientation.Down);

            bool exists = v.RemoveCell(new KeyValuePair<int, int>(2, 3));
            Assert.AreEqual(true, exists);

            exists = v.RemoveCell(new KeyValuePair<int, int>(2, 7));
            Assert.AreEqual(true, exists);

            bool invalid = v.RemoveCell(new KeyValuePair<int, int>(5, 4));
            Assert.AreEqual(false, invalid);

            invalid = v.RemoveCell(new KeyValuePair<int, int>(2, 3)); // already removed
            Assert.AreEqual(false, invalid);
        }
    }
}