using System;
using System.IO;
using System.Text;

namespace Jague.Common.Network
{
    public class DataPacketReader:IDisposable
    {
        private bool disposed = false;

        public uint BytesRemaining => stream?.Remaining() ?? 0u;

        private byte currentBitPosition;
        private byte currentBitValue;
        private readonly Stream stream;

        public DataPacketReader(Stream input)
        {
            stream = input;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                stream?.Dispose();
            }

            disposed = true;
        }
        public bool ReadBit()
        {
            currentBitPosition++;
            if (currentBitPosition > 7)
            {
                currentBitPosition = 0;
                currentBitValue = (byte)stream.ReadByte();
            }

            return ((currentBitValue >> currentBitPosition) & 1) != 0;
        }

        private ulong ReadBits(uint bits)
        {
            ulong value = 0ul;
            for (uint i = 0u; i < bits; i++)
                if (ReadBit())
                    value |= 1ul << (int)i;

            return value;
        }

        public byte ReadByte(uint bits = 8u)
        {
            if (bits > sizeof(byte) * 8)
                throw new ArgumentException();

            return (byte)ReadBits(bits);
        }

        public ushort ReadUShort(uint bits = 16u)
        {
            if (bits > sizeof(ushort) * 8)
                throw new ArgumentException();

            return (ushort)ReadBits(bits);
        }

        public short ReadShort(uint bits = 16u)
        {
            if (bits > sizeof(short) * 8)
                throw new ArgumentException();

            return (short)ReadBits(bits);
        }

        public uint ReadUInt(uint bits = 32u)
        {
            if (bits > sizeof(uint) * 8)
                throw new ArgumentException();

            return (uint)ReadBits(bits);
        }

        public int ReadInt(uint bits = 32u)
        {
            if (bits > sizeof(int) * 8)
                throw new ArgumentException();

            return (int)ReadBits(bits);
        }

        public float ReadSingle(uint bits = 32u)
        {
            if (bits > sizeof(float) * 8)
                throw new ArgumentException();

            int value = (int)ReadBits(bits);
            return BitConverter.Int32BitsToSingle(value);
        }

        public double ReadDouble(uint bits = 64u)
        {
            if (bits > sizeof(double) * 8)
                throw new ArgumentException();

            long value = (long)ReadBits(bits);
            return BitConverter.Int64BitsToDouble(value);
        }

        public ulong ReadULong(uint bits = 64u)
        {
            if (bits > sizeof(ulong) * 8)
                throw new ArgumentException();

            return ReadBits(bits);
        }

        public T ReadEnum<T>(uint bits = 64u) where T : Enum
        {
            if (bits > sizeof(ulong) * 8)
                throw new ArgumentException();

            return (T)Enum.ToObject(typeof(T), ReadBits(bits));
        }

        public byte[] ReadBytes(uint length)
        {
            byte[] data = new byte[length];
            for (uint i = 0u; i < length; i++)
                data[i] = ReadByte();

            return data;
        }

        public string ReadWideStringFixed()
        {
            ushort length = ReadUShort();
            byte[] data = ReadBytes(length * 2u);
            return Encoding.Unicode.GetString(data, 0, data.Length - 2);
        }

        public string ReadWideString()
        {
            bool extended = ReadBit();
            ushort length = (ushort)(ReadUShort(extended ? 15u : 7u) << 1);

            byte[] data = ReadBytes(length);
            return Encoding.Unicode.GetString(data);
        }
    }
}
