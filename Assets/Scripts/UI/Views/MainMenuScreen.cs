using UI.Binders;
using UI.Core;
using UI.Helpers;
using UI.ViewModels;
using UnityEngine;

namespace UI.Views
{
    public class MainMenuScreen : Screen<MainMenuViewModel>
    {
        [SerializeField, AutoBind] private ButtonViewBinder _signUpButton = new();
        [SerializeField, AutoBind] private ButtonViewBinder _signInButton = new();
        [SerializeField, AutoBind] private ButtonViewBinder _playButton = new();
        
        public override void Initialize()
        {
            Bind();
        }
    }
}
