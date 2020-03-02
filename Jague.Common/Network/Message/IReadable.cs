using System;
using System.Collections.Generic;
using System.Text;

namespace Jague.Common.Network.Message
{
    public interface IReadable
    {
        void Read(DataPacketReader reader);
    }
}
