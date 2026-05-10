using System;
using System.Collections.Generic;
using Core.Utils.Logger;
using R3;
using UI.Services.Data;

namespace UI.Services
{
    public class ChatService : IChatService, IDisposable
    {
        private readonly List<ChatMessageData> _messages = new();
        private readonly Subject<ChatMessageData> _subject = new();
        private readonly CompositeDisposable _disposable = new();
        
        public Observable<ChatMessageData> OnMessageAdded => _subject.AsObservable();
        public IReadOnlyCollection<ChatMessageData> Messages => _messages;

        public ChatService(ILoggerService loggerService)
        {
            foreach (var message in loggerService.Messages)
            {
                AddMessage(message);
            }

            loggerService.MessageAdded.Subscribe(AddMessage).AddTo(_disposable);
        }

        public void AddMessage(ChatMessageData data)
        {
            _messages.Add(data);
            _subject.OnNext(data);
        }

        public void Dispose()
        {
            _disposable.Dispose();
            _subject.Dispose();
        }
    }
}
