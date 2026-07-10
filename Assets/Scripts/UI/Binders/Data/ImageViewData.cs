using UnityEngine;

namespace UI.Binders.Data
{
    public readonly struct ImageViewData
    {
        public readonly Sprite Sprite;
        public readonly Color Color;

        public ImageViewData(Sprite sprite, Color color)
        {
            Sprite = sprite;
            Color = color;
        }
    }
}
