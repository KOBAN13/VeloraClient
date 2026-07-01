using R3;

namespace Network.Contracts
{
    public interface ILoginClientService
    {
        Observable<Unit> SuccessLogin { get; }
        Observable<string> LoginErrorRequest { get; }
        string UserName { get; }

        void Login(string username, string password);
    }
}
