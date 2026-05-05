using R3;

namespace Network
{
    public interface IRegisterClientService
    {
        Observable<Unit> SuccessRegister { get; }
        Observable<string> RegisterErrorRequest { get; }
        
        void Register(string username, string password);
    }
}