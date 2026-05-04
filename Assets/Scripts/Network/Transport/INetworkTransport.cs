using System.Threading;
using Cysharp.Threading.Tasks;
using Network.Transport.Data;
using R3;

namespace Network.Transport
{
    public interface INetworkTransport
    {
        Observable<byte[]> Received { get; }
        Observable<Unit> Connected { get; }
        Observable<NetworkDisconnectedData> Disconnected { get; }
        Observable<string> Error { get; }

        UniTask ConnectAsync(CancellationToken token);
        UniTask DisconnectAsync();
        UniTask SendAsync(byte[] data, CancellationToken token);
    }
}
