using System;

namespace UI.Core
{
    public abstract class ViewBinder<TViewModelValue> : ViewBinder
    {
        private ViewModelBinder<TViewModelValue> _viewModelBinder;
        public override Type ValueType => typeof(TViewModelValue);

        public override void Initialize()
        {
            if (ViewModelBinder is ViewModelBinder<TViewModelValue> viewModelBinder)
            {
                _viewModelBinder = viewModelBinder;
                _viewModelBinder.SubscribeParse(Parse);
            }
        }

        public abstract void Parse(TViewModelValue value);
        
        public override void Dispose()
        {
            _viewModelBinder?.DisposeParse();
        }
    }
    
    [Serializable]
    public abstract class ViewBinder : IDisposable
    {
        public ViewModelBinder ViewModelBinder;
        public abstract Type ValueType { get; }

        public abstract void Initialize();
        
        public virtual void Dispose()
        {
            
        }
    }
}
