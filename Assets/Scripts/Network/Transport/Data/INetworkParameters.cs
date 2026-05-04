namespace Network.Transport.Data
{
    public interface INetworkParameters
    {
        ETransportType TransportType { get; }
        string WebsocketUrl { get; }
        string TcpHost { get; }
        int TcpPort { get; }
    }
}
