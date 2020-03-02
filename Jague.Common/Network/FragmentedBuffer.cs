using System;
using System.Collections.Generic;
using System.Text;

namespace Jague.Common.Network
{
    public class FragmentedBuffer
    {
        public bool IsComplete => data.Length == offset;

        public byte[] Data
        {
            get
            {
                if (!IsComplete)
                    throw new InvalidOperationException();
                return data;
            }
        }

        private uint offset;
        private readonly byte[] data;

        public FragmentedBuffer(uint size)
        {
            data = new byte[size];
        }

        public void Populate(DataPacketReader reader)
        {
            if (IsComplete)
                throw new InvalidOperationException();

            uint remaining = reader.BytesRemaining;
            if (remaining < data.Length - offset)
            {
                byte[] newData = reader.ReadBytes(remaining);
                Buffer.BlockCopy(newData, 0, data, (int)offset, (int)remaining);

                offset += (uint)newData.Length;
            }
            else
            {
                byte[] newData = reader.ReadBytes((uint)data.Length - offset);
                Buffer.BlockCopy(newData, 0, data, (int)offset, newData.Length);

                offset += (uint)newData.Length;
            }
        }
    }
}
