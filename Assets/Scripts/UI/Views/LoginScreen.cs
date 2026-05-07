using System;
using R3;
using UI.Binders;
using UI.Core;
using UI.Helpers;
using UI.ViewModels;
using UnityEngine;

namespace UI.Views
{
    public class LoginScreen : Screen<LoginViewModel>
    {
        [SerializeField, AutoBind] private InputFieldTextChangedViewBinder _usernameBinder = new();
        [SerializeField, AutoBind] private InputFieldTextChangedViewBinder _passwordBinder = new();
        [SerializeField, AutoBind] private TextViewBinder _serverStateTextBinder = new();
        [SerializeField, AutoBind] private GameObjectViewBinder _serverStateBannerBinder = new();
        
        [SerializeField, AutoBind] private ButtonViewBinder _loginBinder = new();
        [SerializeField, AutoBind] private ButtonViewBinder _closeBinder = new();

        private IDisposable _interactableSignInButtonSubscription;
        
        public override void Initialize()
        {
            Bind();
            
            _interactableSignInButtonSubscription = ViewModel.InteractableSignInButton
                .Subscribe(isInteractable => _loginBinder.Button.interactable = isInteractable);
        }

        public override void Dispose()
        {
            _interactableSignInButtonSubscription?.Dispose();
            base.Dispose();
        }
    }
}       
