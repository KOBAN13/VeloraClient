using System;
using R3;
using UnityEngine;

namespace UI.Core
{
    public abstract class ViewModel : IDisposable
    {
        protected readonly CompositeDisposable Disposable = new();

        public abstract void Initialize();

        public virtual void Dispose()
        {
            Debug.LogWarning("Disposable");
            
            Disposable.Dispose();
        }
    }
}
