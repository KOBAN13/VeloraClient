using UI.Binders;
using UI.Core;
using UI.Helpers;
using UI.ViewModels;
using UnityEngine;

namespace UI.Views
{
    public class ChatView : Screen<ChatViewModel>
    {
        [SerializeField, AutoBind] private ChatMessageBinder _chatMessageBinder;
        
        public override void Initialize()
        {
            Bind();
        }
    }
}
