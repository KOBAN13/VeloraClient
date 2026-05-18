using Network.Transport.Data;
using ObservableCollections;

namespace Network
{
    public interface IRoomStateService
    { 
        IReadOnlyObservableList<RoomSummaryData> RoomSummaryData { get; }
        R3.Observable<RoomStateData> CurrentRoomChanged { get; }
        R3.Observable<string> ErrorReceived { get; }
        
        RoomStateData CurrentRoom { get; }
        PlayerData LocalPLayer { get; }
        
        bool IsInRoom { get; }
        bool IsOwner { get; }
        bool CanToggleReady { get; }
        bool CanStart { get; }
        
        void RefreshRooms();
        void CreateRoom(string roomName, uint maxPlayers);
        void JoinRoom(ulong roomId);
        void LeaveRoom();
        void SetReady(bool isReady);
        void StartGame();
        void ClearCurrentRoom();
    }
}