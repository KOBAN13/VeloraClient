using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Core.Utils.Pool
{
    [CreateAssetMenu(fileName = "PoolParameters", menuName = "Db/PoolParameters")]
    public class PoolParameters : SerializedScriptableObject, IPoolData
    {
        [OdinSerialize] private Dictionary<EObjectInPoolName, PoolEntry> _pools = new();
        
        public IReadOnlyDictionary<EObjectInPoolName, PoolEntry> ObjectPoolsDictionary => _pools;
    }
}