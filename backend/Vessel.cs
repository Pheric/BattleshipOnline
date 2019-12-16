using System.Collections.Generic;
using System.Linq;

namespace server {
    public class Vessel {
        private readonly Dictionary<KeyValuePair<int, int>, bool> _cells;

        public Vessel(KeyValuePair<int, int> location, int length = 3,
            VesselOrientation orientation = VesselOrientation.Right) {
            Orientation = orientation;
            _cells = GenerateCells(location, length, orientation);
        }

        public VesselOrientation Orientation { get; }
        public Dictionary<KeyValuePair<int, int>, bool> Cells => new Dictionary<KeyValuePair<int, int>, bool>(_cells);
        public int Length => Cells.Count;

        private Dictionary<KeyValuePair<int, int>, bool> GenerateCells(KeyValuePair<int, int> location, int length,
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

        /// <summary>
        /// Checks if all cells in this Vessel are in bounds according to the Board size.
        /// </summary>
        /// <param name="xMax">The size of the board's X axis</param>
        /// <param name="yMax">The size of the board's Y axis</param>
        /// <returns>True if all cells are in bounds, false otherwise</returns>
        public bool AssertPositionInBounds(int xMax, int yMax) {
            return Cells.All(c => c.Key.Key >= 0 && c.Key.Key < xMax && c.Key.Value >= 0 && c.Key.Value < yMax);
        }

        /// <summary>
        /// Use PlayerBoard#StrikeCell().
        /// Called when an opponent is firing on the PlayerBoard.
        /// Marks a cell as hit (true) if it matches the target cell.
        /// </summary>
        /// <param name="cell">The coordinates of the cell to hit</param>
        /// <returns>Whether this Vessel was in that cell, and whether it was then damaged by the
        /// hit (will return false if already struck there)</returns>
        public bool RemoveCell(KeyValuePair<int, int> cell) {
            if (!_cells.TryGetValue(cell, out var cellValue))
                return false;

            _cells[cell] = true;
            return !cellValue;
        }

        /// <summary>
        /// Exports all hit cells. Safe to return to an opponent because all undamaged
        /// cells are not included.
        /// </summary>
        /// <returns>Coordinates to all hit cells</returns>
        public List<KeyValuePair<int, int>> ExportCells() => 
            (from c in _cells where c.Value select c.Key).ToList();

            public bool IsSunk() => _cells.All(c => c.Value);
    }

    public enum VesselOrientation {
        Up,
        Right,
        Down,
        Left
    }

    public enum HitState {
        Missed,
        Hit
    }
}