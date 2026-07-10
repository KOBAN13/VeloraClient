using R3;

namespace Network.Contracts
{
    public interface IReadyToggleCooldownService
    {
        Observable<bool> ReadyToggleInteractable { get; }
        Observable<float> ReadyCooldownProgress { get; }

        bool TrySetReady(bool isReady);
    }
}
