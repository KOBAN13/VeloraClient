using R3;

namespace Network.Contracts
{
    public interface ILoginClientService
    {
        Observable<Unit> SuccessLogin { get; }
        Observable<string> LoginErrorRequest { get; }

        void Login(string username, string password);
    }
}
