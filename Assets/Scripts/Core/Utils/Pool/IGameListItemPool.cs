using Cysharp.Threading.Tasks;
using UI.Views;
using UnityEngine;

namespace Core.Utils.Pool
{
    public interface IGameListItemPool
    {
        UniTask Initialize(GameObject parent);
        GameListItem GetListItem(ulong roomId);
        void ReleaseListItem(ulong roomId);
        GameListItem GetById(ulong roomId);
    }
}
