using System.Collections.Generic;
using server;

namespace api {
    /// <summary>
    /// When clients initially set up their PlayerBoard during the SETUP GameState, this is the
    /// structure of their POST body
    /// </summary>
    public struct SetupRequest {
        public List<Vessel> Vessels;

        public SetupRequest(List<Vessel> vessels) {
            this.Vessels = vessels;
        }
    }
}