using System;
using System.Linq;
using Packets;

namespace Network.Data
{
    public readonly struct RoomSummaryData : IEquatable<RoomSummaryData>, IComparable<RoomSummaryData>
    {
        public readonly ulong RoomId;
        public readonly string RoomName;
        public readonly PlayerData[] Players;
        public readonly uint MaxPlayers;
        public readonly RoomStatus Status;

        public RoomSummaryData(ulong roomId, string roomName, PlayerData[] players, uint maxPlayers, RoomStatus status)
        {
            RoomId = roomId;
            RoomName = roomName ?? string.Empty;
            Players = players ?? Array.Empty<PlayerData>();
            MaxPlayers = maxPlayers;
            Status = status;
        }

        public bool Equals(RoomSummaryData other)
        {
            return RoomId == other.RoomId &&
                   string.Equals(RoomName, other.RoomName, StringComparison.Ordinal) &&
                   GetPlayersOrEmpty(Players).SequenceEqual(GetPlayersOrEmpty(other.Players)) &&
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
                hashCode = (hashCode * 397) ^ (RoomName?.GetHashCode() ?? 0);
                foreach (var player in GetPlayersOrEmpty(Players))
                {
                    hashCode = (hashCode * 397) ^ player.GetHashCode();
                }

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

            var roomNameComparison = string.Compare(RoomName, other.RoomName, StringComparison.Ordinal);
            if (roomNameComparison != 0)
            {
                return roomNameComparison;
            }

            var players = GetPlayersOrEmpty(Players);
            var otherPlayers = GetPlayersOrEmpty(other.Players);
            var playersCountComparison = players.Length.CompareTo(otherPlayers.Length);
            if (playersCountComparison != 0)
            {
                return playersCountComparison;
            }

            var playersComparison = ComparePlayers(players, otherPlayers);
            if (playersComparison != 0)
            {
                return playersComparison;
            }

            var maxPlayersComparison = MaxPlayers.CompareTo(other.MaxPlayers);
            if (maxPlayersComparison != 0)
            {
                return maxPlayersComparison;
            }

            return ((int)Status).CompareTo((int)other.Status);
        }

        private static PlayerData[] GetPlayersOrEmpty(PlayerData[] players)
        {
            return players ?? Array.Empty<PlayerData>();
        }

        private static int ComparePlayers(PlayerData[] left, PlayerData[] right)
        {
            for (var i = 0; i < left.Length; i++)
            {
                var userIdComparison = left[i].UserId.CompareTo(right[i].UserId);
                if (userIdComparison != 0)
                {
                    return userIdComparison;
                }

                var clientIdComparison = left[i].ClientId.CompareTo(right[i].ClientId);
                if (clientIdComparison != 0)
                {
                    return clientIdComparison;
                }

                var usernameComparison = string.Compare(left[i].Username, right[i].Username, StringComparison.Ordinal);
                if (usernameComparison != 0)
                {
                    return usernameComparison;
                }

                var readyComparison = left[i].IsReady.CompareTo(right[i].IsReady);
                if (readyComparison != 0)
                {
                    return readyComparison;
                }

                var ownerComparison = left[i].IsOwner.CompareTo(right[i].IsOwner);
                if (ownerComparison != 0)
                {
                    return ownerComparison;
                }
            }

            return 0;
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
