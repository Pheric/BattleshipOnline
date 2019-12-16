using System;
using System.Collections.Generic;
using System.Linq;

namespace server {
    public class PlayerBoard {
        private List<Vessel> _vessels;

        public PlayerBoard(int rows, int cols, List<int> vesselLengths) {
            Rows = rows;
            Cols = cols;
            VesselLengths = vesselLengths;
            _vessels = new List<Vessel>();
            StrikeRecord = new Dictionary<KeyValuePair<int, int>, HitState>();
        }

        public int Rows { get; }
        public int Cols { get; }

        public List<int> VesselLengths { get; }

        public Dictionary<KeyValuePair<int, int>, HitState> StrikeRecord { get; }

        public List<Vessel> Vessels {
            get => _vessels;
            set {
                if (value.Count != VesselLengths.Count)
                    throw new ArgumentException("Vessel count is invalid for Board configuration");

                var newVessels = new List<Vessel>();

                var cells = new List<KeyValuePair<int, int>>();
                var lengths = new List<int>(VesselLengths);
                foreach (var v in value) {
                    var matchIdx = lengths.FindIndex(val => val == v.Length);

                    if (matchIdx == -1 || !v.AssertPositionInBounds(Rows, Cols))
                        throw new ArgumentException("Vessels are invalid for Board configuration");

                    lengths.RemoveAt(matchIdx);

                    foreach (var cellEntry in v.Cells) {
                        if (cells.Contains(cellEntry.Key))
                            throw new ArgumentException("Vessels must not intersect!");

                        cells.Add(cellEntry.Key);
                    }

                    newVessels.Add(v);
                }

                _vessels = newVessels;
            }
        }

        /// <summary>
        /// Called from the API when the opponent fires upon a cell on this Board.
        /// </summary>
        /// <param name="cell">The coordinates of the target cell</param>
        /// <returns>Whether a vessel was hit</returns>
        public bool StrikeCell(KeyValuePair<int, int> cell) {
            if (StrikeRecord.ContainsKey(cell))
                throw new ArgumentException();
            if (cell.Key < 0 || cell.Value < 0 || cell.Key >= Rows || cell.Value >= Cols)
                throw new ArgumentException();
            
            if (_vessels.Any(v => v.RemoveCell(cell))) {
                this.StrikeRecord.Add(cell, HitState.Hit);
                return true;
            }

            this.StrikeRecord.Add(cell, HitState.Missed);
            return false;
        }

        /// <summary>
        /// Whether this board has been set up
        /// </summary>
        /// <returns>Whether this board has been set up</returns>
        public bool IsSet() => _vessels.Count != 0;

        /// <summary>
        /// Checks whether all Vessels on this PlayerBoard have been sunk.
        /// </summary>
        /// <returns>Whether all Vessels have been sunk</returns>
        public bool IsLost() => _vessels.All(v => v.IsSunk());
    }
}