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
        
        private IDisposable _interactableToggle;

        public override void Initialize()
        {
            Bind();
            
            _interactableToggle = ViewModel.InteractableToggle
                .Subscribe(isInteractable => _readyPlayerToggle.Toggle.interactable = isInteractable);
        }

        public override void Dispose()
        {
            _interactableToggle?.Dispose();
            
            base.Dispose();
        }
    }
}
