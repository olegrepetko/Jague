using Jague.Common.Cryptography;
using Jague.Common.Network.Message;
using Jague.Common.Network.Message.Model;
using Jague.Common.Network.Packet;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Jague.Common.Network
{
    public class BankSession : NetworkSession
    {
        /// <summary>
        /// Determines if queued incoming packets can be processed during a world update.
        /// </summary>
        public bool CanProcessPackets { get; set; } = true;

        private FragmentedBuffer onDeck;
        private readonly ConcurrentQueue<ClientBankPacket> incomingPackets = new ConcurrentQueue<ClientBankPacket>();
        private readonly Queue<ServerBankPacket> outgoingPackets = new Queue<ServerBankPacket>();

        /// <summary>
        /// Enqueue <see cref="IWritable"/> to be sent to the client.
        /// </summary>
        public void EnqueueMessage(IWritable message)
        {
            if (!MessageManager.Instance.GetOpcode(message, out MessageOpcode opcode))
            {
                log.Warn("Failed to send message with no attribute!");
                return;
            }

            if (opcode != MessageOpcode.ServerAuthEncrypted
                && opcode != MessageOpcode.ServerRealmEncrypted)
                log.Trace($"Sent packet {opcode}(0x{opcode:X}).");

            ServerBankPacket packet = new ServerBankPacket(opcode, message);
            outgoingPackets.Enqueue(packet);
        }
       
        public override void OnAccept(Socket newSocket)
        {
            base.OnAccept(newSocket);

            EnqueueMessage(new ServerHello
            {
                AuthVersion = 16042,
                AuthMessage = 0x97998A0,
                ConnectionType = 3
            });
        }
        
        protected override void OnData(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            using (var reader = new DataPacketReader(stream))
            {
                while (stream.Remaining() != 0)
                {
                    // no packet on deck waiting for additional information, new data will be part of a new packet
                    if (onDeck == null)
                    {
                        uint size = reader.ReadUInt();
                        onDeck = new FragmentedBuffer(size - sizeof(uint));
                    }

                    onDeck.Populate(reader);
                    if (onDeck.IsComplete)
                    {
                        var packet = new ClientBankPacket(onDeck.Data);

                        incomingPackets.Enqueue(packet);
                        onDeck = null;
                    }
                }
            }
        }
        
        protected override void OnDisconnect()
        {
            base.OnDisconnect();

            incomingPackets.Clear();
            outgoingPackets.Clear();
        }
        
        public override void Update(double lastTick)
        {
            // process pending packet queue
            while (CanProcessPackets && incomingPackets.TryDequeue(out ClientBankPacket packet))
                HandlePacket(packet);

            // flush pending packet queue
            while (outgoingPackets.TryDequeue(out ServerBankPacket packet))
                FlushPacket(packet);

            base.Update(lastTick);
        }
        
        protected void HandlePacket(ClientBankPacket packet)
        {
            IReadable message = MessageManager.Instance.GetMessage(packet.Opcode);
            if (message == null)
            {
                log.Warn($"Received unknown packet {packet.Opcode:X}");
                return;
            }

            MessageHandlerDelegate handlerInfo = MessageManager.Instance.GetMessageHandler(packet.Opcode);
            if (handlerInfo == null)
            {
                log.Warn($"Received unhandled packet {packet.Opcode}(0x{packet.Opcode:X}).");
                return;
            }

            log.Trace($"Received packet {packet.Opcode}(0x{packet.Opcode:X}).");

            // FIXME workaround for now. possible performance impact. 
            // ClientPing does not currently work and the session times out after 300s -> this keeps the session alive if -any- client packet is received
            Heartbeat.OnHeartbeat();

            using (var stream = new MemoryStream(packet.Data))
            using (var reader = new DataPacketReader(stream))
            {
                message.Read(reader);
                if (reader.BytesRemaining > 0)
                    log.Warn($"Failed to read entire contents of packet {packet.Opcode}");

                try
                {
                    handlerInfo.Invoke(this, message);
                }
                catch (InvalidPacketValueException exception)
                {
                    log.Error(exception);
                    RequestedDisconnect = true;
                }
                catch (Exception exception)
                {
                    log.Error(exception);
                }
            }
        }
        
        [MessageHandler(MessageOpcode.ClientPacked)]
        public void HandlePacked(ClientPacked packed)
        {
            var packet = new ClientBankPacket(packed.Data);
            HandlePacket(packet);
        }
        
        private void FlushPacket(ServerBankPacket packet)
        {
            using (var stream = new MemoryStream())
            using (var writer = new DataPacketWriter(stream))
            {
                writer.Write(packet.Size);
                writer.Write(packet.Opcode, 16);
                writer.WriteBytes(packet.Data);

                SendRaw(stream.ToArray());
            }
        }
    }
}
