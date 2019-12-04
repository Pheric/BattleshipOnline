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
        }

        public int Rows { get; }
        public int Cols { get; }

        public List<int> VesselLengths { get; }

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
            foreach (var v in _vessels) {
                if (v.RemoveCell(cell))
                    return true;
            }

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

        /// <summary>
        /// Exports all hit cells in all Vessels. Safe to return to an opponent because
        /// all undamaged cells are not included.
        /// </summary>
        /// <returns>A List of Vessels, where every Vessel is a List of its damaged cells</returns>
        public List<List<KeyValuePair<int, int>>> ExportVessels() =>
            Vessels.Select(v => v.ExportCells()).ToList();
    }
}