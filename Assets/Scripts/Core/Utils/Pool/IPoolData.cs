using System.Collections.Generic;

namespace Core.Utils.Pool
{
    public interface IPoolData
    {
        IReadOnlyDictionary<EObjectInPoolName, PoolEntry> ObjectPoolsDictionary { get; }
    }
}