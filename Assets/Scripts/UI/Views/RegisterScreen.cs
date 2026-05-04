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
        [SerializeField, AutoBind] private TextViewBinder _errorBinder = new();
        [SerializeField, AutoBind] private GameObjectViewBinder _objectLoginError = new();
        [SerializeField, AutoBind] private ButtonViewBinder _registerBinder = new();
        [SerializeField, AutoBind] private ButtonViewBinder _closeBinder = new();
        
        public override void Initialize()
        {
            Bind();
        }
    }
}