using System;

namespace Jague.Common.Network
{
    public class InvalidPacketValueException : Exception
    {
        public InvalidPacketValueException()
        {
        }

        public InvalidPacketValueException(string message)
            : base(message)
        {
        }
    }
}
