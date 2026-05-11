namespace Network.Transport.Data
{
    public interface INetworkParameters
    {
        ETransportType TransportType { get; }
        string WebsocketUrlInEditor { get; }
        string WebsocketUrlInHttps { get; }
        string TcpHost { get; }
        int TcpPort { get; }
    }
}
