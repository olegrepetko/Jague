using Jague.Common.Network.Message;

namespace Jague.Common.Network.Packet
{
    public abstract class BankPacket
    {
        public const ushort HeaderSize = sizeof(uint) + sizeof(ushort);

        public uint Size { get; protected set; }
        public MessageOpcode Opcode { get; protected set; }

        public byte[] Data { get; protected set; }
    }
}
