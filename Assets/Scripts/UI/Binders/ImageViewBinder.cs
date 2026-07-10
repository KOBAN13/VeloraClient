using System;
using UI.Binders.Data;
using UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Binders
{
    [Serializable]
    public class ImageViewBinder : ViewBinder<ImageViewData>
    {
        [SerializeField] private Image _image;

        public override void Parse(ImageViewData value)
        {
            _image.sprite = value.Sprite;
            _image.color = value.Color;
            _image.enabled = value.Sprite != null;
        }
    }
}
