using System;
using R3;
using UI.Binders;
using UI.Core;
using UI.Helpers;
using UI.ViewModels;
using UnityEngine;

namespace UI.Views
{
    public class PlayerLobbyItem : View<PlayerLobbyItemViewModel>
    {
        [SerializeField, AutoBind] private ButtonViewBinder _kickPlayerButton;
        [SerializeField, AutoBind] private TextViewBinder _userNameText;
        [SerializeField, AutoBind] private GameObjectViewBinder _kickPlayerObject;
        [SerializeField, AutoBind] private ToggleViewBinder _readyPlayerToggle;
        [SerializeField, AutoBind] private ProgressBarViewBinder _readyCooldownProgress;
        [SerializeField, AutoBind] private GraphicViewBinder _readyCooldownObject;
        [SerializeField, AutoBind] private ImageViewBinder _readyStateImage;

        private IDisposable _interactableToggle;
        private IDisposable _readyToggleState;

        public override void Initialize()
        {
            Bind();

            _interactableToggle = ViewModel.ReadyToggleInteractable
                .Subscribe(isInteractable => _readyPlayerToggle.Toggle.interactable = isInteractable);

            _readyToggleState = ViewModel.ReadyToggleState
                .Subscribe(isReady => _readyPlayerToggle.Toggle.SetIsOnWithoutNotify(isReady));
        }

        public override void Dispose()
        {
            _interactableToggle?.Dispose();
            _readyToggleState?.Dispose();

            base.Dispose();
        }
    }
}
