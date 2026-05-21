using R3;

namespace Network.Contracts
{
    public interface IClientIdentityService
    {
        ulong ClientId { get; }
        Observable<ulong> ClientIdChanged { get; }
    }
}
