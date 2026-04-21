using System;
using System.Collections.Generic;
using Core.Utils.Extensions;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.Utils.Addressable
{
    [Serializable]
    public class AddressablePrefabByType<TFilter> where TFilter : class
    {
        [Header("Type")][HideLabel][SerializeField][ValueDropdown(nameof(GetTypes))] private string type;

        [Header("Asset")][HideLabel][SerializeField] private AssetReferenceGameObject asset;
        
        public Type Type => TypeExtensions.GetType(type);
        public AssetReferenceGameObject Asset => asset;

        [UsedImplicitly]
        private IEnumerable<string> GetTypes() => TypeExtensions.FilterTypes<TFilter>();
    }
}