using UI.Binders;
using UI.Core;
using UI.Helpers;
using UI.ViewModels;
using UnityEngine;

namespace UI.Views
{
    public class LobbyScreen : Screen<LobbyViewModel>
    {
        [SerializeField, AutoBind] private ButtonViewBinder _startGameButtonBinder;
        [SerializeField, AutoBind] private ButtonViewBinder _inviteGameButtonBinder;
        [SerializeField, AutoBind] private ButtonViewBinder _leaveGameButtonBinder;
        [SerializeField, AutoBind] private GameObjectViewBinder _objectStartGameButtonBinder;

        [SerializeField] private GameObject _lobbyListContent;

        public override void Initialize()
        {
            Bind();
            ViewModel.SetParentObject.Execute(_lobbyListContent);
        }
    }
}