using UI.Binders;
using UI.Core;
using UI.Helpers;
using UI.ViewModels;
using UnityEngine;

namespace UI.Views
{
    public class ChatView : Screen<ChatViewModel>
    {
        [SerializeField, AutoBind] private ChatMessageBinder _messageAddedBinder = new();
        [SerializeField, AutoBind] private InputFieldTextChangedViewBinder _inputChangedBinder = new();
        [SerializeField, AutoBind] private InputFieldSetTextViewBinder _inputTextBinder = new();
        [SerializeField, AutoBind] private ButtonViewBinder _sendButtonBinder = new();
        
        public override void Initialize()
        {
            Bind();
        }
    }
}
