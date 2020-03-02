using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Jague.Common.Network
{
    public abstract class NetworkSession : Session
    {
        public bool Disconnected { get; private set; }
        public bool RequestedDisconnect { get; protected set; }
        public SocketHeartbeat Heartbeat { get; } = new SocketHeartbeat();

        private Socket socket;
        private readonly byte[] buffer = new byte[4096];

        public virtual void OnAccept(Socket newSocket)
        {
            if (socket != null)
                throw new InvalidOperationException();

            socket = newSocket;
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveDataCallback, null);

            log.Trace("New client connected.");
        }

        public override void Update(double lastTick)
        {
            base.Update(lastTick);

            if (!RequestedDisconnect)
            {
                Heartbeat.Update(lastTick);
                if (Heartbeat.Flatline)
                    RequestedDisconnect = true;
            }
            else if (!Disconnected)
                OnDisconnect();
        }

        protected virtual void OnDisconnect()
        {
            Disconnected = true;
            socket.Close();

            log.Trace("Client disconnected.");
        }

        private void ReceiveDataCallback(IAsyncResult ar)
        {
            try
            {
                int length = socket.EndReceive(ar);
                if (length == 0)
                {
                    RequestedDisconnect = true;
                    return;
                }

                byte[] data = new byte[length];
                Buffer.BlockCopy(buffer, 0, data, 0, data.Length);
                OnData(data);

                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveDataCallback, null);
            }
            catch
            {
                RequestedDisconnect = true;
            }
        }

        protected abstract void OnData(byte[] data);

        protected void SendRaw(byte[] data)
        {
            try
            {
                socket.Send(data, 0, data.Length, SocketFlags.None);
            }
            catch
            {
                RequestedDisconnect = true;
            }
        }
    }
}
