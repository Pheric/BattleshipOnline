using System;
using System.Collections.Generic;
using server;

namespace api {
    public struct PollResponse {
        public Guid Guid { get; }
        public GameState State { get; }
        public List<int> VLengths { get; }
        public List<List<KeyValuePair<int, int>>> ActiveClientVessels { get; }

        public PollResponse(Guid guid, GameState state, List<int> vlengths,
            List<List<KeyValuePair<int, int>>> activeClientVessels) {
            this.Guid = guid;
            this.State = state;
            this.VLengths = vlengths;
            this.ActiveClientVessels = activeClientVessels;
        }
    }
}