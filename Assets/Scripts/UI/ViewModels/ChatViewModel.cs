using R3;
using UI.Core;
using UI.Helpers;
using UI.Services;
using UI.Services.Data;
using VContainer;

namespace UI.ViewModels
{
    public class ChatViewModel : ViewModel
    {
        [AutoBind] private readonly ViewModelBinder<ReactiveCommand<ChatMessageDataView>> _messageAddedBinder = new();
        
        private readonly ReactiveCommand<ChatMessageDataView> _messageAdded = new();
        
        private IChatService _chatService;

        [Inject]
        private void Construct(IChatService chatService)
        {
            _chatService = chatService;
        }
        
        public override void Initialize()
        {
            _messageAddedBinder.Value = _messageAdded;
            
            _chatService.OnMessageAdded.Subscribe(OnMessageReceived).AddTo(Disposable);
        }
        
        private void OnMessageReceived(ChatMessageDataView data)
        {
            _messageAdded.Execute(data);
        }
    }
}
