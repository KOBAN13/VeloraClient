using System.Collections.Generic;
using Packets;

namespace Network.Transport.Data
{
    public class RoomStateData
    {
        public ulong RoomId { get; }
        public uint MaxPlayer { get; }
        public RoomStatus Status { get; }
        public IReadOnlyList<PlayerData> Players { get; }
        
        public RoomStateData(ulong roomId, uint maxPlayer, RoomStatus status, IReadOnlyList<PlayerData> players)
        {
            RoomId = roomId;
            MaxPlayer = maxPlayer;
            Status = status;
            Players = players;
        }
    }
}