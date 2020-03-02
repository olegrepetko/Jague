using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Jague.Common.Network
{
    public class ConnectionListener<T> where T : NetworkSession, new()
    {
        public delegate void NewSessionEvent(T socket);

        public event NewSessionEvent OnNewSession;

        private volatile bool shutdownRequested;

        public ConnectionListener(IPAddress host, uint port)
        {
            var listener = new TcpListener(host, (int)port);
            listener.Start();

            var thread = new Thread(() =>
            {
                while (!shutdownRequested)
                {
                    while (listener.Pending())
                    {
                        var session = new T();
                        session.OnAccept(listener.AcceptSocket());

                        OnNewSession?.Invoke(session);
                    }

                    Thread.Sleep(1);
                }

                listener.Stop();
            });

            thread.Start();
        }

        public void Shutdown()
        {
            shutdownRequested = true;
        }
    }
}
