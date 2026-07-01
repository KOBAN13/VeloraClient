using System;
using R3;
using UI.Binders;
using UI.Core;
using UI.Helpers;
using UI.ViewModels;
using UnityEngine;

namespace UI.Views
{
    public class GameListItem : View<GameListItemViewModel>
    {
        [SerializeField, AutoBind] private ButtonViewBinder _playButtonBinder = new();

        [SerializeField, AutoBind] private TextViewBinder _countPlayerInLobbyBinder = new();

        [SerializeField, AutoBind] private TextViewBinder _roomName = new();

        [SerializeField, AutoBind] private GameObjectViewBinder _lockIcon = new();

        private IDisposable _interactablePlayButton;

        public override void Initialize()
        {
            _interactablePlayButton = ViewModel.InteractablePlayButton.Subscribe(OnInteractablePlayButton);
        }

        private void OnInteractablePlayButton(bool isInteractable)
        {
            _playButtonBinder.Button.interactable = isInteractable;
        }

        public override void Dispose()
        {
            base.Dispose();
            
            _interactablePlayButton.Dispose();
        }
    }
}