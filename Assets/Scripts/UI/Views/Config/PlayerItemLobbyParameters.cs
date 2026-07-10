using UnityEngine;

namespace UI.Views.Config
{
    [CreateAssetMenu(fileName = "PlayerItemLobbyParameters", menuName = "Db/PlayerItemLobbyParameters")]
    public class PlayerItemLobbyParameters : ScriptableObject, IPlayerItemLobbyParameters
    {
        [field: SerializeField] public Sprite PlayerNotReadySprite { get; private set; }
        [field: SerializeField] public Sprite PlayerReadySprite { get; private set; }
        
        [field: SerializeField] public Color PlayerReadyColor { get; private set; }
        [field: SerializeField] public Color PlayerNotReadyColor { get; private set; }
    }
}