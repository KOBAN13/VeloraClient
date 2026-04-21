using UI.Base;
using UI.Binders;
using UI.Helpers;
using UI.ViewModel;
using UnityEngine;

namespace UI.View
{
    public class LoadingScreen : Screen<LoadingViewModel>
    {
        [SerializeField, AutoBind] private ProgressBarViewBinder progressBar = new();
        
        public override void Initialize()
        {
            Bind<RootLifeTimeScope>(progressBar);
        }

        public override void BindGuid()
        {
            UI.ViewModel.ProgressBinder.AutoBindId.SetGuid(progressBar.AutoBindId.GeneratedGuid);
        }
    }
}