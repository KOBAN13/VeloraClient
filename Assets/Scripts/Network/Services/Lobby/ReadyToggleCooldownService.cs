using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Network.Contracts;
using R3;
using UnityEngine;

namespace Network.Services.Lobby
{
    public class ReadyToggleCooldownService : IReadyToggleCooldownService, IDisposable
    {
        private static readonly TimeSpan Cooldown = TimeSpan.FromSeconds(2);

        private readonly ILobbyClientService _lobbyClientService;
        private readonly ReactiveProperty<bool> _readyToggleInteractable = new(true);
        private readonly ReactiveProperty<float> _readyCooldownProgress = new(1f);

        private CancellationTokenSource _cooldownCancellation;
        private float _nextRequestTime;

        public Observable<bool> ReadyToggleInteractable => _readyToggleInteractable;
        public Observable<float> ReadyCooldownProgress => _readyCooldownProgress;

        public ReadyToggleCooldownService(ILobbyClientService lobbyClientService)
        {
            _lobbyClientService = lobbyClientService;
        }

        public bool TrySetReady(bool isReady)
        {
            if (!_readyToggleInteractable.Value || Time.unscaledTime < _nextRequestTime)
            {
                return false;
            }

            _nextRequestTime = Time.unscaledTime + (float)Cooldown.TotalSeconds;
            _lobbyClientService.SetReady(isReady);
            StartCooldown();

            return true;
        }

        public void Dispose()
        {
            CancelCooldown();
            _readyToggleInteractable.Dispose();
            _readyCooldownProgress.Dispose();
        }

        private void StartCooldown()
        {
            CancelCooldown();
            _cooldownCancellation = new CancellationTokenSource();
            RunCooldownAsync(_cooldownCancellation.Token).Forget();
        }

        private async UniTaskVoid RunCooldownAsync(CancellationToken token)
        {
            try
            {
                _readyToggleInteractable.Value = false;
                _readyCooldownProgress.Value = 0f;

                var elapsed = 0f;
                var duration = (float)Cooldown.TotalSeconds;

                while (elapsed < duration)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, token);

                    elapsed += Time.unscaledDeltaTime;
                    _readyCooldownProgress.Value = Math.Min(elapsed / duration, 1f);
                }

                _readyToggleInteractable.Value = true;
                _readyCooldownProgress.Value = 1f;
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void CancelCooldown()
        {
            _cooldownCancellation?.Cancel();
            _cooldownCancellation?.Dispose();
            _cooldownCancellation = null;
        }
    }
}
