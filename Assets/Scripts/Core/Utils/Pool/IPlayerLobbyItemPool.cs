using Cysharp.Threading.Tasks;
using UI.Views;
using UnityEngine;

namespace Core.Utils.Pool
{
    public interface IPlayerLobbyItemPool
    {
        void Initialize(GameObject parent);
        
        PlayerLobbyItem GetListItem(ulong userId);

        void ReleaseListItem(ulong userId);

        PlayerLobbyItem GetById(ulong userId);

        void Clear();
    }
}