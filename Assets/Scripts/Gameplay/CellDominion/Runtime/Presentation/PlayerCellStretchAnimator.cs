using DG.Tweening;
using Gameplay.CellDominion.Runtime.Movement;
using UnityEngine;

namespace Gameplay.CellDominion.Runtime.Presentation
{
    public sealed class PlayerCellStretchAnimator : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D targetBody;
        [SerializeField] private PlayerCellController targetController;
        [SerializeField] private Transform visualRoot;
        [SerializeField] private float minSpeed = 0.08f;
        [SerializeField] private float speedForMaxStretch = 4.5f;
        [SerializeField] private float maxStretch = 0.45f;
        [SerializeField] private float sideCompression = 0.22f;
        [SerializeField] private float scaleTweenDuration = 0.12f;
        [SerializeField] private float rotationTweenDuration = 0.045f;
        [SerializeField] private float retargetInterval = 0.035f;

        private Tween scaleTween;
        private Tween rotationTween;
        private Vector3 lastTargetScale = Vector3.one;
        private float lastTargetRotation;
        private float nextRetargetTime;

        private void Awake()
        {
            if (targetBody == null)
            {
                targetBody = GetComponentInParent<Rigidbody2D>();
            }

            if (targetController == null)
            {
                targetController = GetComponentInParent<PlayerCellController>();
            }

            if (visualRoot == null)
            {
                visualRoot = transform;
            }
        }

        private void LateUpdate()
        {
            if (targetBody == null || visualRoot == null)
            {
                return;
            }

            if (Time.time < nextRetargetTime)
            {
                return;
            }

            nextRetargetTime = Time.time + retargetInterval;

            Vector2 velocity = targetBody.linearVelocity;
            float speed = velocity.magnitude;
            Vector2 aimDirection = targetController == null ? Vector2.zero : targetController.AimDirection;
            Vector2 facingDirection = aimDirection.sqrMagnitude > 0.0001f ? aimDirection : velocity.normalized;

            if (speed <= minSpeed)
            {
                if (facingDirection.sqrMagnitude > 0.0001f)
                {
                    float idleRotation = Mathf.Atan2(facingDirection.y, facingDirection.x) * Mathf.Rad2Deg;
                    ApplyTarget(Vector3.one, idleRotation);
                }
                else
                {
                    ApplyTarget(Vector3.one, lastTargetRotation);
                }

                return;
            }

            float normalizedSpeed = Mathf.Clamp01(speed / Mathf.Max(0.01f, speedForMaxStretch));
            float stretch = maxStretch * normalizedSpeed;
            float compression = sideCompression * normalizedSpeed;
            Vector3 targetScale = new Vector3(1f + stretch, 1f - compression, 1f);
            float targetRotation = Mathf.Atan2(facingDirection.y, facingDirection.x) * Mathf.Rad2Deg;

            ApplyTarget(targetScale, targetRotation);
        }

        private void OnDestroy()
        {
            scaleTween?.Kill();
            rotationTween?.Kill();
        }

        private void ApplyTarget(Vector3 targetScale, float targetRotation)
        {
            if ((lastTargetScale - targetScale).sqrMagnitude > 0.0001f)
            {
                scaleTween?.Kill();
                scaleTween = visualRoot.DOScale(targetScale, scaleTweenDuration).SetEase(Ease.OutSine);
                lastTargetScale = targetScale;
            }

            if (Mathf.Abs(Mathf.DeltaAngle(lastTargetRotation, targetRotation)) > 0.5f)
            {
                rotationTween?.Kill();
                rotationTween = visualRoot.DOLocalRotate(new Vector3(0f, 0f, targetRotation), rotationTweenDuration).SetEase(Ease.OutSine);
                lastTargetRotation = targetRotation;
            }
        }
    }
}
