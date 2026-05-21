using System.Collections.Generic;

namespace Network.Transport
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
