using Jague.Common.Network.Message;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jague.Common.Network.Packet
{
    public class ClientBankPacket:BankPacket
    {
        public ClientBankPacket(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            using (var reader = new DataPacketReader(stream))
            {
                Opcode = (MessageOpcode)reader.ReadUShort();
                Data = reader.ReadBytes(reader.BytesRemaining);
                Size = (uint)Data.Length + HeaderSize;
            }
        }
    }
}
