using Packets;

namespace Network.Transport.Data
{
    public readonly struct RoomSummaryData
    {
        public readonly ulong RoomId;
        public readonly uint PlayersCount;
        public readonly uint MaxPlayers;
        public readonly RoomStatus Status;

        public RoomSummaryData(ulong roomId, uint playersCount, uint maxPlayers, RoomStatus status)
        {
            RoomId = roomId;
            PlayersCount = playersCount;
            MaxPlayers = maxPlayers;
            Status = status;
        }
    }
}