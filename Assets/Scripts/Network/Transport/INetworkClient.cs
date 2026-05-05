using Cysharp.Threading.Tasks;
using Network.Transport.Data;
using Packets;
using R3;

namespace Network.Transport
{
    public interface INetworkClient
    {
        Observable<Packet> Received { get; }
        Observable<Unit> Connected { get; }
        Observable<NetworkDisconnectedData> Disconnected { get; }
        Observable<string> Error { get; }
        
        UniTaskVoid SendAsync(Packet packet);
    }
}
