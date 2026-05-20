using R3;

namespace Network
{
    public interface IClientIdentityService
    {
        ulong ClientId { get; }
        Observable<ulong> ClientIdChanged { get; }
    }
}
