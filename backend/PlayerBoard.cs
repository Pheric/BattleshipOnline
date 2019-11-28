using System;
using System.Collections.Generic;

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

        public bool StrikeCell(KeyValuePair<int, int> cell) {
            foreach (var v in _vessels) {
                if (v.RemoveCell(cell))
                    return true;
            }

            return false;
        }

        public List<Dictionary<KeyValuePair<int, int>, bool>> ExportVessels() {
            var vessels = new List<Dictionary<KeyValuePair<int, int>, bool>>();
            foreach (var v in Vessels)
                vessels.Add(v.ExportCells());

            return vessels;
        }
    }
}