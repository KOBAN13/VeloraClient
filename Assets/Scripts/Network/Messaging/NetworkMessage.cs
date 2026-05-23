using Packets;

namespace Network.Messaging
{
    public readonly struct NetworkMessage<T> where T : class
    {
        public T Payload { get; }
        public ulong SenderId { get; }
        public Packet RawPacket { get; }

        public NetworkMessage(T payload, ulong senderId, Packet rawPacket)
        {
            Payload = payload;
            SenderId = senderId;
            RawPacket = rawPacket;
        }
    }
}
