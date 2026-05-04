namespace Network.Transport.Data
{
    public readonly struct NetworkDisconnectedData
    {
        public readonly int Code;
        public readonly string Reason;

        public NetworkDisconnectedData(int code, string reason)
        {
            Code = code;
            Reason = reason;
        }
    }
}
