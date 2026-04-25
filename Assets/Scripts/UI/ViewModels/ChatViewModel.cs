using R3;
using UI.Core;
using UI.Helpers;
using UI.Services;
using UI.Services.Data;
using UnityEngine;
using VContainer;

namespace UI.ViewModels
{
    public class ChatViewModel : ViewModel
    {
        [AutoBind] private readonly ViewModelBinder<ReactiveCommand<ChatMessageDataView>> _messageAddedBinder = new();
        [AutoBind] private readonly ViewModelBinder<ReactiveCommand<string>> _inputChangedBinder = new();
        [AutoBind] private readonly ViewModelBinder<string> _inputTextBinder = new();
        [AutoBind] private readonly ViewModelBinder<ReactiveCommand> _sendButtonBinder = new();
        
        private readonly ReactiveCommand<ChatMessageDataView> _messageAdded = new();
        private readonly ReactiveCommand<string> _inputChanged = new();
        private readonly ReactiveCommand _sendButtonClicked = new();
        
        private IChatService _chatService;
        private string _inputText = string.Empty;

        [Inject]
        private void Construct(IChatService chatService)
        {
            _chatService = chatService;
        }
        
        public override void Initialize()
        {
            _messageAddedBinder.Value = _messageAdded;
            _inputChangedBinder.Value = _inputChanged;
            _inputTextBinder.Value = string.Empty;
            _sendButtonBinder.Value = _sendButtonClicked;
            
            _chatService.OnMessageAdded.Subscribe(OnMessageReceived).AddTo(Disposable);
            _inputChanged.Subscribe(OnInputChanged).AddTo(Disposable);
            _sendButtonClicked.Subscribe(_ => SendCurrentMessage()).AddTo(Disposable);
        }
        
        private void OnMessageReceived(ChatMessageDataView data)
        {
            _messageAdded.Execute(data);
        }

        private void OnInputChanged(string value)
        {
            _inputText = value;
        }

        private void SendCurrentMessage()
        {
            if (string.IsNullOrWhiteSpace(_inputText))
                return;

            var message = _inputText.Trim();

            _chatService.SendMessage(new ChatMessageData("Player", message, Color.white));

            _inputText = string.Empty;
            _inputTextBinder.Value = string.Empty;
        }
    }
}
