using System.Collections.Generic;

namespace Core.Utils.Services
{
    public class TickService
    {
        private readonly List<ITickable> _tickables = new();
        
        public void RegisterTick(ITickable tickable)
        {
            _tickables.Add(tickable);
        }

        public void Tick(float deltaTime)
        {
            foreach (var tickable in _tickables)
            {
                tickable.Tick(deltaTime);
            }
        }

        public void UnregisterTick(ITickable tickable)
        {
            _tickables.Remove(tickable);
        }
    }
}
