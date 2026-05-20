using System;

namespace Network.Transport.Data
{
    public readonly struct PlayerData : IEquatable<PlayerData>
    {
        public readonly ulong UserId;
        public readonly ulong ClientId;
        public readonly string Username;
        public readonly bool IsReady;
        public readonly bool IsOwner;

        public PlayerData(ulong userId, ulong clientId, string username, bool isReady, bool isOwner)
        {
            UserId = userId;
            ClientId = clientId;
            Username = username;
            IsReady = isReady;
            IsOwner = isOwner;
        }

        public static bool operator ==(PlayerData a, PlayerData b)
        {
            return a.Equals(b);
        }
        
        public static bool operator !=(PlayerData a, PlayerData b)
        {
            return !a.Equals(b);
        }


        public bool Equals(PlayerData other)
        {
            return UserId == other.UserId && ClientId == other.ClientId && Username == other.Username && IsReady == other.IsReady && IsOwner == other.IsOwner;
        }

        public override bool Equals(object obj)
        {
            return obj is PlayerData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UserId, ClientId, Username, IsReady, IsOwner);
        }
    }
}