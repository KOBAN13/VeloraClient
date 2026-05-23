using Packets;
using R3;

namespace Network.Messaging.Stream
{
    public class MessageStream<T> : IMessageStream where T : class
    {
        private readonly Subject<NetworkMessage<T>> _subject = new();

        public Observable<NetworkMessage<T>> Observable => _subject;
        
        public void Publish(object payload, ulong senderId, Packet rawPacket)
        {
            var typedPayload = payload as T;
            
            _subject.OnNext(new NetworkMessage<T>(typedPayload, senderId, rawPacket));
        }

        public void Dispose()
        {
            _subject.Dispose();
        }
    }
}