using System;
using System.Collections.Generic;
using Core.Utils.Logger;
using Core.Utils.Services;
using Cysharp.Threading.Tasks;
using Network.Messaging.Stream;
using Network.Transport.Contracts;
using Packets;
using R3;

namespace Network.Messaging
{
    public class NetworkMessageBus : INetworkMessageBus, IInitializable, IDisposable
    {
        private readonly INetworkClient _networkClient;
        private readonly ILoggerService _logger;
        private readonly Dictionary<Type, IMessageStream> _streams = new();
        private readonly CompositeDisposable _disposables = new();

        public bool IsInitialized { get; set; }

        public NetworkMessageBus(INetworkClient networkClient, ILoggerService logger)
        {
            _networkClient = networkClient;
            _logger = logger;
        }

        public void Initialize()
        {
            _networkClient.Received
                .Subscribe(OnPacketReceived)
                .AddTo(_disposables);
        }
        
        public Observable<NetworkMessage<T>> On<T>() where T : class
        {
            return GetStream<T>().Observable;
        }

        public UniTaskVoid Send<T>(T data) where T : class
        {
            return _networkClient.SendAsync(PacketMessageMap.CreatePacket(data));
        }

        public void Dispose()
        {
            _disposables.Dispose();

            foreach (var stream in _streams.Values)
            {
                stream.Dispose();
            }

            _streams.Clear();
            IsInitialized = false;
        }

        private void OnPacketReceived(Packet packet)
        {
            if (packet.MsgCase == Packet.MsgOneofCase.None)
            {
                _logger.Warning("Received packet without message payload.", nameof(NetworkMessageBus));
                return;
            }

            if (!PacketMessageMap.TryExtract(packet, out var messageType, out var payload))
            {
                _logger.Warning($"Unable to extract payload for {packet.MsgCase}.", nameof(NetworkMessageBus));
                return;
            }

            Publish(messageType, payload, packet);
        }
        
        private void Publish(Type messageType, object payload, Packet rawPacket)
        {
            var stream = _streams[messageType];
            
            stream.Publish(payload, rawPacket.SenderId, rawPacket);
        }

        private MessageStream<T> GetStream<T>() where T : class
        {
            var type = typeof(T);

            if (_streams.TryGetValue(type, out var stream))
            {
                return (MessageStream<T>)stream;
            }

            var messageStream = new MessageStream<T>();
            _streams.Add(type, messageStream);

            return messageStream;
        }
    }
}
