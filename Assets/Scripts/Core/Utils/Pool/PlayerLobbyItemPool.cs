using System.Collections.Generic;
using System.Linq;
using Core.Utils.Data;
using Core.Utils.Factory;
using Cysharp.Threading.Tasks;
using UI.Views;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using VContainer;

namespace Core.Utils.Pool
{
    public class PlayerLobbyItemPool : IPlayerLobbyItemPool
    {
        [Inject] private ScreensData _screensData;
        [Inject] private ViewsFactory _viewsFactory;

        private readonly Dictionary<ulong, PlayerLobbyItem> _activePlayers = new();
        private ObjectPool<PlayerLobbyItem> _pool;
        private PlayerLobbyItem _prefab;
        private GameObject _parent;

        public void Initialize(GameObject parent)
        {
            _parent = parent;

            Clear();
            _pool?.Clear();

            _pool = new ObjectPool<PlayerLobbyItem>
            (
                OnCreateGameListItem,
                OnGetGameListItem,
                OnReleaseGameListItem,
                OnDestroyGameListItem,
                true,
                10,
                100
            );
        }

        private static void OnDestroyGameListItem(PlayerLobbyItem obj)
        {
            Addressables.Release(obj.gameObject);
        }

        private static void OnReleaseGameListItem(PlayerLobbyItem obj)
        {
            obj.gameObject.SetActive(false);
        }

        private static void OnGetGameListItem(PlayerLobbyItem obj)
        {
            obj.gameObject.SetActive(true);
        }

        private PlayerLobbyItem OnCreateGameListItem()
        {
            var gameListItem = _viewsFactory.Create(_prefab, _parent.transform);
            gameListItem.gameObject.SetActive(false);
            return gameListItem;
        }

        public PlayerLobbyItem GetListItem(ulong userId)
        {
            var item = _pool.Get();
            _activePlayers[userId] = item;
            return item;
        }

        public void ReleaseListItem(ulong userId)
        {
            if (!_activePlayers.Remove(userId, out var item))
                return;

            _pool.Release(item);
        }

        public PlayerLobbyItem GetById(ulong userId)
        {
            _activePlayers.TryGetValue(userId, out var item);
            return item;
        }

        public void Clear()
        {
            if (_pool == null)
            {
                _activePlayers.Clear();
                return;
            }

            foreach (var kv in _activePlayers.Values)
                _pool.Release(kv);

            _activePlayers.Clear();
        }
    }
}