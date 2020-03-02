using System;
using System.Collections.Generic;
using System.Text;

namespace Jague.Common.Network.Message
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MessageAttribute : Attribute
    {
        public MessageOpcode Opcode { get; }

        public MessageAttribute(MessageOpcode opcode)
        {
            Opcode = opcode;
        }
    }
}
