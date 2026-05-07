using System;
using R3;
using UI.Binders;
using UI.Core;
using UI.Helpers;
using UI.ViewModels;
using UnityEngine;

namespace UI.Views
{
    public class RegisterScreen : Screen<RegisterViewModel>
    {
        [SerializeField, AutoBind] private InputFieldTextChangedViewBinder _loginBinder = new();
        [SerializeField, AutoBind] private InputFieldTextChangedViewBinder _passwordBinder = new();
        [SerializeField, AutoBind] private TextViewBinder _serverStateTextBinder = new();
        [SerializeField, AutoBind] private GameObjectViewBinder _serverStateBannerBinder = new();
        [SerializeField, AutoBind] private ButtonViewBinder _registerBinder = new();
        [SerializeField, AutoBind] private ButtonViewBinder _closeBinder = new();

        private IDisposable _interactableRegisterButtonSubscription;
        
        public override void Initialize()
        {
            Bind();
            _interactableRegisterButtonSubscription = ViewModel.InteractableRegisterButton
                .Subscribe(isInteractable => _registerBinder.Button.interactable = isInteractable);
        }

        public override void Dispose()
        {
            _interactableRegisterButtonSubscription?.Dispose();
            base.Dispose();
        }
    }
}
