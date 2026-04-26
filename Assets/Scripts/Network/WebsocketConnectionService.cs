using System;
using Core.Utils.Extensions;
using Core.Utils.Logger;
using Core.Utils.Services;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using NativeWebSocket;
using Packets;
using UnityEngine;

namespace Network
{
    public class WebsocketConnectionService : IInitializable, ITickable, IDisposable, IWebsocketConnectionService
    {
        private WebSocket _webSocket;
        private readonly ILoggerService _logger;

        public bool IsInitialized { get; set; }

        public WebsocketConnectionService(ILoggerService logger)
        {
            _logger = logger;
        }

        public void Initialize()
        {
            CreateWebSocket().Forget();
        }
        
        public async void Dispose()
        {
            if (_webSocket != null)
            {
                await _webSocket.Close();
            }
        }

        public void PrepareNewPackage(string newMsg)
        {
            var chat = new ChatMessage
            {
                Msg = newMsg
            };

            var packet = new Packet
            {
                Chat = chat
            };
            
            var bytes = packet.ToByteArray();

            _webSocket.Send(bytes);
        }

        public void Tick(float deltaTime)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            _webSocket?.DispatchMessageQueue();
#endif
        }

        private async UniTaskVoid CreateWebSocket()
        {
            _webSocket = new WebSocket("ws://localhost:8080/velora");

            _webSocket.OnOpen += OnOpenWebSocketConnection;
            _webSocket.OnMessage += OnMessageWebSocket;
            _webSocket.OnError += OnWebSocketError;
            _webSocket.OnClose += OnWebSocketClose;
            
            _logger.Log("Connecting...");

            await _webSocket.Connect();
        }
        
        private void OnOpenWebSocketConnection()
        {
            _logger.Log("Connect successfully!");
        }
        
        private void OnMessageWebSocket(byte[] data)
        {
            Debug.LogError("Пришло сообщение!");
            
            try
            {
                var packet = Packet.Parser.ParseFrom(data);
                
                if (packet.Id != null)
                    HandleIdMessage(packet.SenderId, packet.Id);       
                else if (packet.Chat != null)
                    HandleChatMessage(packet.SenderId, packet.Chat);
            }
            catch (InvalidProtocolBufferException e)
            {
                Debug.LogError(
                    "Parse error: payload is not a complete Packet protobuf. " +
                    $"bytes={data.Length}, hex={data.ToHexString()}, utf8={data.ToUtf8Preview()}. Exception: {e}");
            }
            catch (Exception e)
            {
                Debug.LogError("Parse error: " + e);
            }
        }

        private void HandleIdMessage(ulong senderId, IdMessage msg)
        {
            var clientId = msg.Id;
            
            _logger.Log("Received clientId: " + clientId);
        }

        private void HandleChatMessage(ulong senderId, ChatMessage msg)
        {
            _logger.Log($"Client {senderId} : {msg.Msg}");
        }

        private void OnWebSocketError(string errorMsg)
        {
            _logger.Error($"WebSocket error: {errorMsg}");
        }
        
        private void OnWebSocketClose(WebSocketCloseCode closeCode)
        {
            _logger.Warning($"WebSocket closed with code {closeCode}");
        }
    }
}
