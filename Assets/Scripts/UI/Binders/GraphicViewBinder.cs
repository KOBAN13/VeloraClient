using System;
using UI.Core;
using UI.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Binders
{
    [Serializable]
    public class GraphicViewBinder : ViewBinder<EUIObjectState>
    {
        [SerializeField] private Graphic _graphic;

        public override void Parse(EUIObjectState value)
        {
            switch (value)
            {
                case EUIObjectState.Show:
                    _graphic.enabled = true;
                    break;
                case EUIObjectState.Hide:
                    _graphic.enabled = false;
                    break;
                case EUIObjectState.None:
                    break;
            }
        }
    }
}
