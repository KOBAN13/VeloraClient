using System;
using R3;
using UI.Binders;
using UI.Core;
using UI.Helpers;
using UI.ViewModels;
using UnityEngine;

namespace UI.Views
{
    public class CreateRoomScreen : Screen<CreateRoomViewModel>
    {
        [SerializeField, AutoBind] private InputFieldTextChangedViewBinder _roomNameTextViewBinder;
        [SerializeField, AutoBind] private InputFieldTextChangedViewBinder _roomMaxPlayersTextViewBinder;
        [SerializeField, AutoBind] private ButtonViewBinder _createRoomButtonBinder;
        [SerializeField, AutoBind] private ButtonViewBinder _closeScreenButtonBinder;
        
        private IDisposable _interactableCreateRoomButton;

        public override void Initialize()
        {
            Bind();

            _interactableCreateRoomButton = ViewModel.InteractableCreateRoomButton.Subscribe(InteractableCreateRoomButton);
        }

        private void InteractableCreateRoomButton(bool interactable)
        {
            _createRoomButtonBinder.Button.interactable = interactable;
        }

        public override void Dispose()
        {
            base.Dispose();
            _interactableCreateRoomButton?.Dispose();
        }
    }
}