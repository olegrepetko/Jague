using Jague.Common.Network;
using Jague.Common.Network.Message;
using System;

namespace Jague.BankServer.Network.Message.Model
{
    [Message(MessageOpcode.ClientHello)]
    public class ClientHello : IReadable
    {
        public uint Build { get; private set; }
        public string Email { get; private set; }
        public NetworkGuid ServerToken { get; } = new NetworkGuid();

        public void Read(DataPacketReader reader)
        {
            Build = reader.ReadUInt();
            Email = reader.ReadWideStringFixed();
            ServerToken.Read(reader);
        }
    }
}
