using System;
using System.Collections.Generic;
using System.Text;

namespace Jague.Common.Network.Message.Model
{
    [Message(MessageOpcode.ServerHello)]
    public class ServerHello : IWritable
    {
        public uint AuthVersion { get; set; }
        public ulong StartupTime { get; set; }

        public void Write(DataPacketWriter writer)
        {
            writer.Write(AuthVersion);
            writer.Write(StartupTime);
        }
    }
}
