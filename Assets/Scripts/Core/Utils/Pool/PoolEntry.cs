using System;
using UnityEngine;

namespace Core.Utils.Pool
{
    [Serializable]
    public class PoolEntry
    {
        public GameObject prefab;
        public int prewarmCount = 10;
        public Type objectType;
    }
}