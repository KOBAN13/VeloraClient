using System;
using Core.Utils.Factory;
using UI.Helpers;
using UnityEngine;
using VContainer;

namespace UI.Core
{
    public abstract class View<TViewModel> : View
        where TViewModel : ViewModel, new()
    {
        [Inject] private ViewModelFactory _viewModelFactory;

        private ViewBinder[] _viewBinders;
        public TViewModel ViewModel { get; private set; }
        
        protected void Bind()
        {
            _viewBinders = AutoBindResolver.GetViewBinders(this);
            
            ViewModel = _viewModelFactory.Create<TViewModel>();

            ViewModel.Initialize();
            
            AutoBindResolver.Bind(this, ViewModel);

            foreach (var viewBinder in _viewBinders)
                viewBinder.Initialize();
        }

        public override void ApplyPayload<TPayload>(TPayload payload)
        {
            base.ApplyPayload(payload);

            if (ViewModel is IPayloadReceiver<TPayload> viewModelReceiver)
            {
                viewModelReceiver.ApplyPayload(payload);
            }
        }

        public override void Dispose()
        {
            foreach (var viewBinder in _viewBinders)
            {
                viewBinder.Dispose();
            }
        }
    }

    public abstract class View : MonoBehaviour, IDisposable
    {
        public abstract void Initialize();

        public virtual void Open()
        {
            gameObject.SetActive(true);
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
        }

        public virtual void ApplyPayload<TPayload>(TPayload payload)
        {
            if (this is IPayloadReceiver<TPayload> payloadReceiver)
            {
                payloadReceiver.ApplyPayload(payload);
            }
        }

        public virtual void Dispose()
        {
        }
    }
}
