using System;
using System.Collections.Generic;
using System.Text;

namespace Jague.Common.Network.Message
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MessageHandlerAttribute : Attribute
    {
        public MessageOpcode Opcode { get; }

        public MessageHandlerAttribute(MessageOpcode opcode)
        {
            Opcode = opcode;
        }
    }
}
