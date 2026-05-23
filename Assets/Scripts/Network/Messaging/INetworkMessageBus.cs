using Cysharp.Threading.Tasks;
using R3;

namespace Network.Messaging
{
    public interface INetworkMessageBus
    {
        Observable<NetworkMessage<T>> On<T>() where T : class;
        UniTaskVoid Send<T>(T data) where T : class;
    }
}
