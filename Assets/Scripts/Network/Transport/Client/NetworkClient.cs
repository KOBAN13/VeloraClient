using System;
using System.Threading;
using Core.Utils.Logger;
using Core.Utils.Services;
using Cysharp.Threading.Tasks;
using Network.Transport.Data;
using Packets;
using R3;

namespace Network.Transport
{
    public class NetworkClient : INetworkClient, IInitializable, IDisposable
    {
        private readonly INetworkTransport _transport;
        private readonly IPacketCodec _packetCodec;
        private readonly IMessageFramer _messageFramer;
        private readonly ILoggerService _logger;
        private readonly CompositeDisposable _disposable = new();
        private readonly Subject<Packet> _received = new();
        private readonly Subject<string> _error = new();
        
        private readonly CancellationTokenSource _token = new();

        public bool IsInitialized { get; set; }
        
        public Observable<Packet> Received => _received;
        public Observable<Unit> Connected => _transport.Connected;
        public Observable<NetworkDisconnectedData> Disconnected => _transport.Disconnected;
        public Observable<string> Error => _error;

        public NetworkClient(
            INetworkTransport transport,
            IPacketCodec packetCodec,
            IMessageFramer messageFramer,
            ILoggerService logger)
        {
            _transport = transport;
            _packetCodec = packetCodec;
            _messageFramer = messageFramer;
            _logger = logger;
        }
        
        public void Initialize()
        {
            _transport.Received.Subscribe(OnTransportReceived).AddTo(_disposable);
            _transport.Error.Subscribe(PublishError).AddTo(_disposable);
        }

        public UniTask DisconnectAsync()
        {
            return _transport.DisconnectAsync();
        }

        public async UniTaskVoid SendAsync(Packet packet)
        {
            try
            {
                _token.Token.ThrowIfCancellationRequested();

                var payload = _packetCodec.Encode(packet);
                var frame = _messageFramer.WriteFrame(payload);

                await _transport.SendAsync(frame, _token.Token);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                PublishError($"Packet send error: {e.Message}");
                throw;
            }
        }

        public void Dispose()
        {
            _disposable.Dispose();
            _received.Dispose();
            _error.Dispose();
        }

        private void OnTransportReceived(byte[] data)
        {
            try
            {
                foreach (var frame in _messageFramer.ReadFrames(data))
                {
                    if (_packetCodec.TryDecode(frame, out var packet, out var error))
                    {
                        _received.OnNext(packet);
                        continue;
                    }

                    PublishError(error);
                }
            }
            catch (Exception e)
            {
                PublishError($"Packet receive error: {e.Message}");
            }
        }

        private void PublishError(string error)
        {
            if (string.IsNullOrEmpty(error))
                return;

            _logger.Error(error, nameof(NetworkClient));
            _error.OnNext(error);
        }
    }
}
