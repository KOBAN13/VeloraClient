using Packets;
using R3;

namespace Network
{
    public interface ILobbyClientService
    {
        Observable<RoomListSnapshotMessage> RoomListSnapshotReceived { get; }
        Observable<RoomStateSnapshotMessage> RoomStateSnapshotReceived { get; }
        Observable<string> LobbyErrorReceived { get; }

        void RefreshRooms();
        void CreateRoom(string nameRoom, uint maxPlayers);
        void LeaveRoom();
        void JoinRoom(uint roomId);
        void SetReady(bool isReady);
        void StartGame();
    }
}