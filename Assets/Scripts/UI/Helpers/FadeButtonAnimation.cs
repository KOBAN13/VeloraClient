using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace UI.Helpers
{
    public class FadeButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [field: SerializeField] public float OnClickAlpha { get; private set; }
        [field: SerializeField] public float FadeTime { get; private set; }
        [field: SerializeField] public float OnHoverAlpha { get; private set; }
        
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
    }
}