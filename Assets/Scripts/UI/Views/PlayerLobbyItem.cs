using UI.Binders;
using UI.Core;
using UI.Helpers;
using UI.ViewModels;
using UnityEngine;

namespace UI.Views
{
    public class PlayerLobbyItem : View<PlayerLobbyItemViewModel>
    {
        [SerializeField, AutoBind] private ButtonViewBinder _kickPlayerButton;
        [SerializeField, AutoBind] private TextViewBinder _userNameText;
        [SerializeField, AutoBind] private GameObjectViewBinder _kickPlayerObject;

        public override void Initialize()
        {
            Bind();
        }
    }
}