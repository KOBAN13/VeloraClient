using System;
using Packets;

namespace Network.Messaging.Stream
{
    public interface IMessageStream : IDisposable
    {
        void Publish(object payload, ulong senderId, Packet rawPacket);
    }
}