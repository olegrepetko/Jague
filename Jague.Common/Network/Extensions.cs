using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jague.Common.Network
{
    public static class Extensions
    {
        public static uint Remaining(this Stream stream)
        {
            if (stream.Length < stream.Position)
                throw new InvalidOperationException();

            return (uint)(stream.Length - stream.Position);
        }
    }
}
