using System;
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

            await _webSocket.Connect();
        }
        
        private void OnOpenWebSocketConnection()
        {
            var request = new Packet
            {
                Chat = new ChatMessage
                {
                    Msg = "Hello from Unity!"
                },
                SenderId = 500
            };

            var bytes = request.ToByteArray();
            
            Debug.Log(
                $"bytes={bytes.Length}, hex={bytes.ToHexString()}, utf8={bytes.ToUtf8Preview()}.");
            
            _webSocket.Send(bytes);
        }
        
        private void OnMessageWebSocket(byte[] data)
        {
            Debug.Log("Received bytes: " + data.Length);

            try
            {
                var packet = Packet.Parser.ParseFrom(data);
                Debug.Log($"Server response: senderId={packet.SenderId}, payload={packet.DescribePacket()}");
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

        private void OnWebSocketError(string errorMsg)
        {
            Debug.LogError(errorMsg);
        }
        
        private void OnWebSocketClose(WebSocketCloseCode closeCode)
        {
            Debug.Log(closeCode.ToString());
        }
    }
}
