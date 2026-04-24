using System;
using Core.Utils.Logger;
using Cysharp.Threading.Tasks;
using Extensions;
using Google.Protobuf;
using NativeWebSocket;
using Packets;
using Unity.VisualScripting;
using UnityEngine;
using VContainer.Unity;
using IInitializable = Unity.VisualScripting.IInitializable;

namespace Network
{
    public class WebsocketConnectionService : IInitializable, ITickable, IDisposable, IWebsocketConnectionService
    {
        private WebSocket _webSocket;
        private ILoggerService _logger;

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

        public void Tick()
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
            
            var request = new Packet
            {
                Chat = new ChatMessage
                {
                    Msg = "Hello from Unity!"
                },
                SenderId = 500
            };

            var bytes = request.ToByteArray();
            
            _webSocket.Send(bytes);
        }
        
        private void OnMessageWebSocket(byte[] data)
        {
            try
            {
                var packet = Packet.Parser.ParseFrom(data);
                var senderId = packet.SenderId;
                
                if (senderId != 0)
                    HandleIdMessage(senderId, packet.Id);
                else
                    HandleChatMessage(senderId, packet.Chat);
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
            _logger.Log($"Client {senderId} received message: {msg.Msg}");
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
