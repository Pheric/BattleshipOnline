using System;
using System.Collections.Generic;
using server;

namespace api {
    public struct PollResponse {
        public GameState State { get; }
        public Dictionary<KeyValuePair<int, int>, HitState> ActiveClientVessels { get; }

        public PollResponse(GameState state, Dictionary<KeyValuePair<int, int>, HitState> activeClientVessels) {
            this.State = state;
            this.ActiveClientVessels = activeClientVessels;
        }
    }
}