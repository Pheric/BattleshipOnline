using System.Collections.Generic;
using System.Linq;

namespace server {
    public class Vessel {
        private readonly Dictionary<KeyValuePair<int, int>, bool> _cells;

        public Vessel(KeyValuePair<int, int> location, int length = 3,
            VesselOrientation orientation = VesselOrientation.Right) {
            Orientation = orientation;
            _cells = generateCells(location, length, orientation);
        }

        public VesselOrientation Orientation { get; }
        public Dictionary<KeyValuePair<int, int>, bool> Cells => new Dictionary<KeyValuePair<int, int>, bool>(_cells);
        public int Length => Cells.Count;

        private Dictionary<KeyValuePair<int, int>, bool> generateCells(KeyValuePair<int, int> location, int length,
            VesselOrientation orientation) {
            var cells = new Dictionary<KeyValuePair<int, int>, bool>();
            switch (orientation) {
                case VesselOrientation.Up:
                    for (int x = location.Key, y = location.Value; location.Value - y < length; y--)
                        cells.Add(new KeyValuePair<int, int>(x, y), false);
                    break;
                case VesselOrientation.Right:
                    for (int x = location.Key, y = location.Value; x - location.Key < length; x++)
                        cells.Add(new KeyValuePair<int, int>(x, y), false);
                    break;
                case VesselOrientation.Down:
                    for (int x = location.Key, y = location.Value; y - location.Value < length; y++)
                        cells.Add(new KeyValuePair<int, int>(x, y), false);
                    break;
                case VesselOrientation.Left:
                    for (int x = location.Key, y = location.Value; location.Key - x < length; x--)
                        cells.Add(new KeyValuePair<int, int>(x, y), false);
                    break;
            }

            return cells;
        }

        public bool AssertPositionInBounds(int xMax, int yMax) {
            return Cells.All(c => c.Key.Key >= 0 && c.Key.Key < xMax && c.Key.Value >= 0 && c.Key.Value < yMax);
        }

        public bool RemoveCell(KeyValuePair<int, int> cell) {
            if (_cells.ContainsKey(cell)) {
                var ret = !_cells[cell];
                _cells[cell] = true;

                return ret;
            }

            return false;
        }

        public Dictionary<KeyValuePair<int, int>, bool> exportCells() {
            var cells = new Dictionary<KeyValuePair<int, int>, bool>();
            foreach (var c in Cells)
                if (c.Value)
                    cells.Add(c.Key, true);

            return cells;
        }
    }

    public enum VesselOrientation {
        Up,
        Right,
        Down,
        Left
    }
}