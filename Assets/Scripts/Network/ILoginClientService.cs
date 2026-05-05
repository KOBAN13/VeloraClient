using R3;

namespace Network
{
    public interface ILoginClientService
    {
        Observable<Unit> SuccessLogin { get; }
        Observable<string> LoginErrorRequest { get; }

        void Login(string username, string password);
    }
}
