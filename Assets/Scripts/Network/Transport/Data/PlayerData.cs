namespace Network.Transport.Data
{
    public readonly struct PlayerData
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
    }
}