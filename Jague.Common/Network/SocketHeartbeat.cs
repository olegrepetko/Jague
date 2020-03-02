using System;
using System.Collections.Generic;
using System.Text;

namespace Jague.Common.Network
{
    public class SocketHeartbeat : IUpdate
    {
        public bool Flatline => timeToFlatline <= 0d;

        private double timeToFlatline;

        public SocketHeartbeat()
        {
            OnHeartbeat();
        }

        public void OnHeartbeat()
        {
            timeToFlatline = 300d;
        }

        public void Update(double lastTick)
        {
            timeToFlatline -= lastTick;
        }
    }
}
