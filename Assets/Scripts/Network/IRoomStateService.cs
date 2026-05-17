using System.Collections.Generic;
using Network.Transport.Data;
using R3;

namespace Network
{
    public interface IRoomStateService
    {
        Observable<IReadOnlyList<RoomSummaryData>> RoomsChanged { get; }
        Observable<RoomStateData> CurrentRoomChanged { get; }
        Observable<string> ErrorReceived { get; }
        
        IReadOnlyList<RoomSummaryData> Rooms { get; }
        RoomStateData CurrentRoom { get; }
        PlayerData LocalPLayer { get; }
        
        bool IsInRoom { get; }
        bool IsOwner { get; }
        bool CanToggleReady { get; }
        bool CanStart { get; }
        
        void RefreshRooms();
        void CreateRoom(uint maxPlayers);
        void JoinRoom(ulong roomId);
        void LeaveRoom();
        void SetReady(bool isReady);
        void StartGame();
        void ClearCurrentRoom();
    }
}