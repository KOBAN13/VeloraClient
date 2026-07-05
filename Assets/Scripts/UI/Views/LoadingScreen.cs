using UI.Binders;
using UI.Core;
using UI.Helpers;
using UI.ViewModels;
using UnityEngine;

namespace UI.Views
{
    public class LoadingScreen : Screen<LoadingViewModel>
    {
        [SerializeField, AutoBind] private ProgressBarViewBinder _progressBarBinder = new();
        
        public override void Initialize()
        {
            Bind();
        }
    }
}
