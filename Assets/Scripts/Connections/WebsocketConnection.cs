using System;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using NativeWebSocket;
using Packets;
using UnityEngine;

namespace Connections
{
    public class WebsocketConnection : MonoBehaviour
    {
        private WebSocket _webSocket;

        private async void Awake()
        {
            CreateWebSocket().Forget();
        }

        private void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            _webSocket?.DispatchMessageQueue();
#endif
        }

        private async void OnApplicationQuit()
        {
            if (_webSocket != null)
            {
                await _webSocket.Close();
            }
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
            var chatMessage = new ChatMessage
            {
                Msg = "Hello from Unity!"
            };

            var id = new IdMessage
            {
                Id = 150
            };

            var request = new Packet
            {
                Chat = chatMessage,
                Id = id,
                SenderId = 500
            };

            var bytes = request.ToByteArray();
            
            Debug.Log("Sending bytes: " + bytes.Length);
            
            _webSocket.Send(bytes);
        }
        
        private void OnMessageWebSocket(byte[] data)
        {
            Debug.Log("Received bytes: " + data.Length);

            try
            {
                var packet = Packet.Parser.ParseFrom(data);
                Debug.Log($"Server response: id={packet.Id}, message={packet.Chat.Msg}");
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
