using UI.Binders;
using UI.Core;
using UI.Helpers;
using UI.ViewModels;
using UnityEngine;

namespace UI.Views
{
    public class GameRoomHubScreen : Screen<GameRoomHubViewModel>
    {
        [SerializeField, AutoBind] private TextViewBinder _userNameBinder;
        [SerializeField, AutoBind] private ButtonViewBinder _createRoomButtonBinder;
        [SerializeField, AutoBind] private ButtonViewBinder _logoutButtonBinder;
        [SerializeField, AutoBind] private ButtonViewBinder _autoLobbyButtonBinder;
        
        [SerializeField] private GameObject _lobbyList;
        
        public override void Initialize()
        {
            Bind();
            ViewModel.SetParentObject.Execute(_lobbyList);
        }
    }
}