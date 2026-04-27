using Core.Utils.Pool;
using UI.Binders;
using UI.Core;
using UI.Helpers;
using UI.ViewModels;
using UnityEngine;
using VContainer;

namespace UI.Views
{
    public class ChatScreen : Screen<ChatViewModel>
    {
        [SerializeField, AutoBind] private ChatMessageBinder _messageAddedBinder = new();
        [SerializeField, AutoBind] private InputFieldTextChangedViewBinder _inputChangedBinder = new();
        [SerializeField, AutoBind] private InputFieldSetTextViewBinder _inputTextBinder = new();
        [SerializeField, AutoBind] private ButtonViewBinder _sendButtonBinder = new();

        private IPoolService _poolService;

        [Inject]
        private void Construct(IPoolService poolService)
        {
            _poolService = poolService;
        }
        
        public override void Initialize()
        {
            _messageAddedBinder.SetPoolService(_poolService);
            Bind();
        }
    }
}
