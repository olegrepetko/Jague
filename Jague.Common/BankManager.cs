using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Jague.Common
{
    public sealed class BankManager : Singleton<BankManager>
    {
        private volatile bool shutdownRequested;

        private BankManager()
        {
        }

        public void Initialise(Action<double> updateAction)
        {
            var worldThread = new Thread(() =>
            {
                var stopwatch = new Stopwatch();
                double lastTick = 0d;

                while (!shutdownRequested)
                {
                    stopwatch.Restart();

                    updateAction(lastTick);

                    Thread.Sleep(1);
                    lastTick = (double)stopwatch.ElapsedTicks / Stopwatch.Frequency;
                }
            });

            worldThread.Start();
        }

        public void Shutdown()
        {
            shutdownRequested = true;
        }
    }
}
