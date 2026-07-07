using Network.Data;
using ObservableCollections;
using Packets;
using R3;

namespace Network.Contracts
{
    public interface ILobbyClientService
    {
        Observable<RoomListSnapshotMessage> RoomListSnapshotReceived { get; }
        Observable<RoomStateSnapshotMessage> RoomStateSnapshotReceived { get; }
        Observable<PlayersInRoomResponse> PlayersInRoomReceived { get; }
        Observable<string> LobbyErrorReceived { get; }
        IReadOnlyObservableList<PlayerData> Players { get; }
        Observable<Unit> KickedUser { get; }

        void GetPlayersInLobby(ulong roomId);
        void RefreshRooms();
        void CreateRoom(string nameRoom, uint maxPlayers);
        void LeaveRoom();
        void JoinRoom(ulong roomId);
        void SetReady(bool isReady);
        void StartGame();
        void KickUser(ulong userId);
    }
}