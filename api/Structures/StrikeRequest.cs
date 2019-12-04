using System.Collections.Generic;

namespace api {
    /// <summary>
    /// When clients wish to strike a cell, this is the structure of their
    /// POST body
    /// </summary>
    public struct StrikeRequest {
        public KeyValuePair<int, int> Coordinate;

        public StrikeRequest(KeyValuePair<int, int> coordinate) {
            this.Coordinate = coordinate;
        }
    }
}