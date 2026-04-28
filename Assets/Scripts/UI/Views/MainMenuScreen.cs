using UI.Binders;
using UI.Core;
using UI.Helpers;
using UI.ViewModels;
using UnityEngine;

namespace UI.Views
{
    public class MainMenuScreen : Screen<LoadingViewModel>
    {
        [SerializeField, AutoBind] private ButtonViewBinder _signUpButton = new();
        [SerializeField, AutoBind] private ButtonViewBinder _signInButton = new();
        [SerializeField, AutoBind] private ButtonViewBinder _playButton = new();
        [SerializeField, AutoBind] private TextViewBinder _usernameBinder = new();
        [SerializeField, AutoBind] private GameObjectViewBinder _objectLoginPanel = new();
        [SerializeField, AutoBind] private GameObjectViewBinder _objectUsernamePanel = new();
        [SerializeField, AutoBind] private ButtonViewBinder _autoLoginButton = new();
        
        public override void Initialize()
        {
            Bind();
        }
    }
}