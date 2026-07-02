using Network.Transport.Data;
using R3;

namespace Network.Contracts
{
    public interface ISessionManager
    {
        Observable<ERoomRole> MyRole { get; }
        T RoomVariables<T>(string keyVariable);
        T UserVariables<T>(string keyVariable);
        void SetRole(ERoomRole role);
        ERoomRole GetRole();
        int FindUserIdByName(string name);
    }
}