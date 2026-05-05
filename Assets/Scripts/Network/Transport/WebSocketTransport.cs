using System;
using System.Threading;
using Core.Utils.Logger;
using Core.Utils.Services;
using Cysharp.Threading.Tasks;
using NativeWebSocket;
using Network.Transport.Data;
using R3;

namespace Network.Transport
{
    public class WebSocketTransport : IInitializable, ITickable, IDisposable, INetworkTransport
    {
        private WebSocket _webSocket;
        private UniTaskCompletionSource _connectCompletionSource;
        private readonly ILoggerService _logger;
        private readonly INetworkParameters _networkParameters;

        public Observable<byte[]> Received => _received;
        public Observable<Unit> Connected => _connected;
        public Observable<NetworkDisconnectedData> Disconnected => _disconnected;
        public Observable<string> Error => _error;

        private readonly Subject<byte[]> _received = new();
        private readonly Subject<Unit> _connected = new();
        private readonly Subject<NetworkDisconnectedData> _disconnected = new();
        private readonly Subject<string> _error = new();

        public bool IsInitialized { get; set; }

        public WebSocketTransport(ILoggerService logger, INetworkParameters networkParameters)
        {
            _logger = logger;
            _networkParameters = networkParameters;
        }

        public void Initialize()
        {
            InitializeAsync().Forget();
        }
        
        public async void Dispose()
        {
            await DisconnectAsync();

            _received.Dispose();
            _connected.Dispose();
            _disconnected.Dispose();
            _error.Dispose();
        }

        public void Tick(float deltaTime)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            _webSocket?.DispatchMessageQueue();
#endif
        }
        
        private void CreateWebSocket()
        {
            _webSocket = new WebSocket(_networkParameters.WebsocketUrl);
            _webSocket.OnOpen += OnOpenWebSocketConnection;
            _webSocket.OnMessage += OnMessageWebSocket;
            _webSocket.OnError += OnWebSocketError;
            _webSocket.OnClose += OnWebSocketClose;
        }
        
        private async UniTaskVoid InitializeAsync()
        {
            try
            {
                await ConnectAsync(CancellationToken.None);
            }
            catch (Exception e)
            {
                PublishError($"WebSocket initialize error: {e.Message}");
            }
        }

        public async UniTask ConnectAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (_webSocket != null)
            {
                switch (_webSocket.State)
                {
                    case WebSocketState.Open:
                        return;
                    case WebSocketState.Connecting:
                        await _connectCompletionSource.Task.AttachExternalCancellation(token);
                        return;
                }
            }

            CreateWebSocket();
            
            _connectCompletionSource = new UniTaskCompletionSource();

            await using var cancellation = token.Register(CancelConnection);

            _logger.Log($"Connecting to {_networkParameters.WebsocketUrl}", nameof(WebSocketTransport));
            
            RunWebSocketAsync(_webSocket).Forget();

            await _connectCompletionSource.Task;
        }

        public async UniTask DisconnectAsync()
        {
            try
            {
                _webSocket.CancelConnection();
                
                await _webSocket.Close();
            }
            catch (Exception e)
            {
                PublishError($"WebSocket disconnect error: {e.Message}");
            }
        }

        public async UniTask SendAsync(byte[] data, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (_webSocket.State != WebSocketState.Open)
                throw new InvalidOperationException("WebSocket is not connected.");

            try
            {
                await _webSocket.Send(data).AsUniTask().AttachExternalCancellation(token);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                PublishError($"WebSocket send error: {e.Message}");
                throw;
            }
        }
        
        private async UniTaskVoid RunWebSocketAsync(WebSocket webSocket)
        {
            try
            {
                await webSocket.Connect();
            }
            catch (Exception e)
            {
                PublishError($"WebSocket connect error: {e.Message}");
                _connectCompletionSource?.TrySetException(e);
            }
        }

        private void OnOpenWebSocketConnection()
        {
            _connected.OnNext(Unit.Default);
            _connectCompletionSource?.TrySetResult();
            _logger.Log("Connected successfully", nameof(WebSocketTransport));
        }
        
        private void OnMessageWebSocket(byte[] data)
        {
            _received.OnNext(data);
        }

        private void OnWebSocketError(string errorMsg)
        {
            PublishError($"WebSocket error: {errorMsg}");
            _connectCompletionSource?.TrySetException(new Exception(errorMsg));
        }

        private void OnWebSocketClose(WebSocketCloseCode closeCode)
        {
            var data = new NetworkDisconnectedData((int)closeCode, closeCode.ToString());
            _disconnected.OnNext(data);
            _connectCompletionSource?.TrySetException(new Exception($"WebSocket closed with code {closeCode}"));
            _logger.Warning($"Closed with code {closeCode}", nameof(WebSocketTransport));
        }

        private void CancelConnection()
        {
            try
            {
                _webSocket?.CancelConnection();
                _connectCompletionSource?.TrySetCanceled();
            }
            catch (Exception e)
            {
                PublishError($"WebSocket cancel error: {e.Message}");
            }
        }

        private void PublishError(string error)
        {
            _logger.Error(error, nameof(WebSocketTransport));
            _error.OnNext(error);
        }

        private void UnsubscribeWebSocketEvents()
        {
            _webSocket.OnOpen -= OnOpenWebSocketConnection;
            _webSocket.OnMessage -= OnMessageWebSocket;
            _webSocket.OnError -= OnWebSocketError;
            _webSocket.OnClose -= OnWebSocketClose;
        }
    }
}
