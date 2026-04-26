using System.Collections.Generic;
using Core.Utils.Addressable;
using Sirenix.OdinInspector;
using UI.Core;
using UnityEngine;

namespace Core.Utils.Data
{
    [CreateAssetMenu(fileName = "SceneConfig", menuName = "Db/SceneConfig")]
    public class ScreensData : SerializedScriptableObject
    {
        [field: SerializeField] public Canvas Canvas { get; private set; }
        
        [SerializeField] private List<AddressablePrefabByType<View>> _screens;

        public IReadOnlyList<AddressablePrefabByType<View>> Screens => _screens;
    }
}