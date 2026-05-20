using System;
using Packets;

namespace Network.Transport.Data
{
    public readonly struct RoomSummaryData : IEquatable<RoomSummaryData>, IComparable<RoomSummaryData>
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

        public bool Equals(RoomSummaryData other)
        {
            return RoomId == other.RoomId &&
                   PlayersCount == other.PlayersCount &&
                   MaxPlayers == other.MaxPlayers &&
                   Status == other.Status;
        }

        public override bool Equals(object obj)
        {
            return obj is RoomSummaryData other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = RoomId.GetHashCode();
                hashCode = (hashCode * 397) ^ PlayersCount.GetHashCode();
                hashCode = (hashCode * 397) ^ MaxPlayers.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)Status;
                return hashCode;
            }
        }

        public int CompareTo(RoomSummaryData other)
        {
            var roomIdComparison = RoomId.CompareTo(other.RoomId);
            if (roomIdComparison != 0)
            {
                return roomIdComparison;
            }

            var playersCountComparison = PlayersCount.CompareTo(other.PlayersCount);
            if (playersCountComparison != 0)
            {
                return playersCountComparison;
            }

            var maxPlayersComparison = MaxPlayers.CompareTo(other.MaxPlayers);
            if (maxPlayersComparison != 0)
            {
                return maxPlayersComparison;
            }

            return ((int)Status).CompareTo((int)other.Status);
        }

        public static bool operator ==(RoomSummaryData left, RoomSummaryData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RoomSummaryData left, RoomSummaryData right)
        {
            return !left.Equals(right);
        }
    }
}
