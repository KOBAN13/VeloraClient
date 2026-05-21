using R3;

namespace Network.Contracts
{
    public interface IRegisterClientService
    {
        Observable<Unit> SuccessRegister { get; }
        Observable<string> RegisterErrorRequest { get; }
        
        void Register(string username, string password);
    }
}