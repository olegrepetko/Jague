using System;
using System.IO;

namespace Jague.Common.Network
{
    public class DataPacketWriter : IDisposable
    {
        private bool disposed = false;

        private byte bitPosition;
        private byte bitValue;
        private readonly Stream stream;

        public DataPacketWriter(Stream output)
        {     
            stream = output;
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
        public void FlushBits()
        {
            if (bitPosition == 0)
                return;

            stream.WriteByte(bitValue);

            bitPosition = 0;
            bitValue = 0;
        }
        public void Write(bool value)
        {
            if (value)
                bitValue |= (byte)(1 << bitPosition);

            bitPosition++;
            if (bitPosition == 8)
                FlushBits();
        }
        public void Write(byte value, uint bits = 8u)
        {
            if (bits > sizeof(byte) * 8)
                throw new ArgumentException();

            WriteBits(value, bits);
        }
        private void WriteBits(ulong value, uint bits)
        {
            for (int i = 0; i < bits; i++)
                Write(Convert.ToBoolean((value >> i) & 1));
        }
        public void Write(ulong value, uint bits = 64)
        {
            if (bits > sizeof(double) * 8)
                throw new ArgumentException();

            WriteBits(value, bits);
        }
        public void Write(int value, uint bits = 32u)
        {
            if (bits > sizeof(uint) * 8)
                throw new ArgumentException();

            WriteBits((uint)value, bits);
        }
        public void Write<T>(T value, uint bits = 64u) where T : Enum, IConvertible
        {
            if (bits > sizeof(ulong) * 8)
                throw new ArgumentException();

            WriteBits(value.ToUInt64(null), bits);
        }
        public void WriteBytes(byte[] data, uint length = 0u)
        {
            if (length != 0 && length != data.Length)
                throw new ArgumentException();

            foreach (byte value in data)
                WriteBits(value, 8);
        }

    }
}
