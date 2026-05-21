using System.Collections.Generic;

namespace Network.Transport
{
    public interface IMessageFramer
    {
        IEnumerable<byte[]> ReadFrames(byte[] data);
        byte[] WriteFrame(byte[] payload);
    }
}
