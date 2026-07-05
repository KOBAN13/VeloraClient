using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Helpers
{
    public class FadeButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [field: SerializeField] public float OnClickAlpha { get; private set; } = 0.6f;
        [field: SerializeField] public float FadeTime { get; private set; } = 0.2f;
        [field: SerializeField] public float OnHoverAlpha { get; private set; } = 0.4f;
        
        private Tween _tween;
        private CanvasGroup _group;
        
        private void Start()
        {
            _group = GetComponent<CanvasGroup>();
            
            if (_group == null)
                _group = gameObject.AddComponent<CanvasGroup>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _tween?.Kill();
            _tween = _group.DOFade(OnHoverAlpha, FadeTime);
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            _tween?.Kill();
            _tween = _group.DOFade(1.0f, FadeTime);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _group.alpha = OnClickAlpha;
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            _group.alpha = 1.0f;
        }

        private void OnValidate()
        {
            OnClickAlpha = 0.6f;
            FadeTime = 0.2f;
            OnHoverAlpha = 0.4f;
        }
    }
}