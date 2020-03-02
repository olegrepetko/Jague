using Jague.Common.Network.Message;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jague.Common.Network.Packet
{
    public class ServerBankPacket : BankPacket
    {
        public ServerBankPacket(MessageOpcode opcode, IWritable message)
        {
            using (var stream = new MemoryStream())
            using (var writer = new DataPacketWriter(stream))
            {
                message.Write(writer);
                writer.FlushBits();
                Data = stream.ToArray();
            }

            Opcode = opcode;
            Size = (ushort)(HeaderSize + Data.Length);
        }
    }
}
