using System;
using System.Collections.Generic;
using Core.Utils.Factory;
using Cysharp.Threading.Tasks;
using UI.Core;
using UnityEngine;
using VContainer;

namespace Core.Utils.Screens
{
    public class ScreenService : IScreenService
    {
        [Inject] private readonly ScreensFactory _screensFactory;
        
        private readonly Dictionary<Type, View> _screensByType = new();
        private readonly LinkedList<View> _screens = new();
        
        public View CurrentScreen { get; private set; }

        public async UniTask<TScreen> OpenAsync<TScreen>() where TScreen : View
        {
            return await OpenAsyncInternal<TScreen, object>(default, false);
        }

        public async UniTask<TScreen> OpenAsync<TScreen, TPayload>(TPayload payload) where TScreen : View
        {
            return await OpenAsyncInternal<TScreen, TPayload>(payload, true);
        }

        public async UniTask PreloadAsync(IProgress<float> progress = null, params Type[] screenTypes)
        {
            var targets = screenTypes is { Length: > 0 }
                ? screenTypes
                : _screensFactory.ScreenTypes;

            if (targets.Count == 0)
            {
                progress?.Report(1f);
                return;
            }

            var completed = 0;

            foreach (var screenType in targets)
            {
                await PreloadAsync(screenType);
                completed++;
                progress?.Report(completed / (float)targets.Count);
            }
        }

        public TScreen OpenSync<TScreen>() where TScreen : View
        {
            return OpenSyncInternal<TScreen, object>(default, false);
        }

        public TScreen OpenSync<TScreen, TPayload>(TPayload payload) where TScreen : View
        {
            return OpenSyncInternal<TScreen, TPayload>(payload, true);
        }

        public void ClearCollections()
        {
            foreach (var screen in _screensByType.Values)
            {
                screen.Dispose();
                UnityEngine.Object.Destroy(screen.gameObject);
            }

            _screensByType.Clear();
            _screens.Clear();
            CurrentScreen = null;
        }

        public void CloseScreen<TScreen>() where TScreen : View
        {
            if (_screensByType.TryGetValue(typeof(TScreen), out var screen))
            {
                RemoveFromStack(screen);
                screen.Close();
                screen.gameObject.SetActive(false);
                
                CurrentScreen = _screens.Count > 0 ? _screens.Last.Value : null;
            }
        }

        public void Close()
        {
            if (_screens.Count == 0)
                return;

            var screen = _screens.Last.Value;
            
            _screens.RemoveLast();
            screen.Close();
            screen.gameObject.SetActive(false);
            
            CurrentScreen = _screens.Count > 0 ? _screens.Last.Value : null;
        }

        private async UniTask<TScreen> OpenAsyncInternal<TScreen, TPayload>(TPayload payload, bool hasPayload)
            where TScreen : View
        {
            if (_screensByType.TryGetValue(typeof(TScreen), out var cachedScreen))
            {
                var typedScreen = (TScreen)cachedScreen;

                if (hasPayload)
                {
                    typedScreen.ApplyPayload(payload);
                }

                return OpenScreen(typedScreen, false);
            }

            var newScreen = await _screensFactory.CreateAsync<TScreen>();

            if (hasPayload)
            {
                newScreen.ApplyPayload(payload);
            }

            return OpenScreen(newScreen, true);
        }

        private async UniTask PreloadAsync(Type screenType)
        {
            if (_screensByType.ContainsKey(screenType))
                return;

            var screen = await _screensFactory.CreateAsync(screenType);
            _screensByType.Add(screenType, screen);
        }

        private TScreen OpenSyncInternal<TScreen, TPayload>(TPayload payload, bool hasPayload) where TScreen : View
        {
            if (_screensByType.TryGetValue(typeof(TScreen), out var cachedScreen))
            {
                var typedScreen = (TScreen)cachedScreen;

                if (hasPayload)
                {
                    typedScreen.ApplyPayload(payload);
                }

                return OpenScreen(typedScreen, false);
            }

            var newScreen = _screensFactory.CreateSync<TScreen>();

            if (hasPayload)
            {
                newScreen.ApplyPayload(payload);
            }

            return OpenScreen(newScreen, true);
        }

        private TScreen OpenScreen<TScreen>(TScreen screen, bool addToCache) where TScreen : View
        {
            screen.Open();
            screen.transform.SetAsLastSibling();

            if (addToCache)
            {
                _screensByType.Add(typeof(TScreen), screen);
            }
            else
            {
                RemoveFromStack(screen);
            }

            _screens.AddLast(screen);
            CurrentScreen = screen;

            return screen;
        }

        private void RemoveFromStack(View screen)
        {
            while (_screens.Remove(screen))
            {
            }
        }
    }
}
