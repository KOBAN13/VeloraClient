using System.Collections.Generic;
using VContainer.Unity;
using ITickable = Core.Utils.Services.ITickable;

namespace Core.Utils
{
    public class TickService
    {
        private List<ITickable> _tickables = new();
        
        public void RegisterTick(ITickable tickable)
        {
            _tickables.Add(tickable);
        }

        public void Tick(float deltaTime)
        {
            for (int i = 0; i < _tickables.Count; i++)
            {
                _tickables[i].Tick(deltaTime);
            }
        }

        public void UnregisterTick(ITickable tickable)
        {
            _tickables.Remove(tickable);
        }
    }
}