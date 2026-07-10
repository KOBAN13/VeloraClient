using UnityEngine;

namespace UI.Views.Config
{
    public interface IPlayerItemLobbyParameters
    { 
        Sprite PlayerNotReadySprite { get; }
        Sprite PlayerReadySprite { get; }
        
        Color PlayerReadyColor { get; }
        Color PlayerNotReadyColor { get; }
    }
}