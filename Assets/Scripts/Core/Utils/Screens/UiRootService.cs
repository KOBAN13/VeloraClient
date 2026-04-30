using Core.Utils.Data;
using Core.Utils.Services;
using UnityEngine;

namespace Core.Utils.Screens
{
    public class UiRootService : IUiRootService, IInitializable
    {
        public bool IsInitialized { get; set; }
        public Transform Root { get; private set; }
        
        private readonly ScreensData _screensData;

        public UiRootService(ScreensData screensData)
        {
            _screensData = screensData;
        }
        
        public void Initialize()
        {
            var canvas = Object.Instantiate(_screensData.Canvas, null);
            
            Object.DontDestroyOnLoad(canvas);
            
            Root = canvas.transform;
        }
    }
}