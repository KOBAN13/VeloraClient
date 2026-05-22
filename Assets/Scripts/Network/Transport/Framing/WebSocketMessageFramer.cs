using System.Collections.Generic;
using Network.Transport.Contracts;

namespace Network.Transport.Framing
{
    public class WebSocketMessageFramer : IMessageFramer
    {
        public IEnumerable<byte[]> ReadFrames(byte[] data)
        {
            yield return data;
        }

        public byte[] WriteFrame(byte[] payload)
        {
            return payload;
        }
    }
}
