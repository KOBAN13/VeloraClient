using UnityEngine;

namespace Core.Utils.Pool
{
    public interface IPoolService
    {
        T Spawn<T>(EObjectInPoolName id, bool isActive) where T : Component;
        T Spawn<T>(EObjectInPoolName id, bool isActive, Vector3 position) where T : Component;
        void ReturnToPool<T>(EObjectInPoolName id, T obj) where T : Component;
    }
}