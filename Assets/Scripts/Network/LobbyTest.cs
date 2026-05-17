using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace Network
{
    public class LobbyTest : MonoBehaviour
    {
        [Inject] private ILobbyClientService _service;

        [Button]
        void CreateGame()
        {
            _service.CreateRoom("test_game", 4);
        }

        [Button]
        void JoinGame()
        {
            _service.JoinRoom(1);
        }
        
        [Button]
        void LeaveGame()
        {
            _service.LeaveRoom();
        }
        
        [Button]
        void StartGame()
        {
            _service.StartGame();
        }

        [Button]
        void SetReady()
        {
            _service.SetReady(true);
        }
    }
}