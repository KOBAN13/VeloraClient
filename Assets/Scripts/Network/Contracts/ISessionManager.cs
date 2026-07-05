using Network.Transport.Data;
using R3;

namespace Network.Contracts
{
    public interface ISessionManager
    {
        Observable<ERoomRole> MyRole { get; }
        T RoomVariables<T>(string keyVariable) where T : new();
        T UserVariables<T>(string keyVariable) where T : new();
        void SetRole(ERoomRole role);
        ERoomRole GetRole();
        ulong FindUserIdByName(string name);
    }
}