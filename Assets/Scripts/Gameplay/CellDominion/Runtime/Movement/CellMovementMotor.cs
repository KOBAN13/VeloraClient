using UnityEngine;

namespace Gameplay.CellDominion.Runtime.Movement
{
    public sealed class CellMovementMotor
    {
        private Vector2 velocity;
        private float dashCooldown;

        public Vector2 Velocity
        {
            get
            {
                return velocity;
            }
        }

        public void Reset(Vector2 initialVelocity)
        {
            velocity = initialVelocity;
            dashCooldown = 0f;
        }

        public Vector2 Tick(Vector2 inputDirection, CellMovementStats stats, float deltaTime)
        {
            Vector2 normalizedInput = inputDirection.sqrMagnitude > 1f ? inputDirection.normalized : inputDirection;
            float dashInterval = Mathf.Max(0.01f, stats.DashInterval);
            float dashControl = Mathf.Clamp01(stats.DashControl);
            dashCooldown -= deltaTime;

            if (normalizedInput.sqrMagnitude > 0.0001f)
            {
                Vector2 targetVelocity = normalizedInput * stats.MaxSpeed;
                float controlAcceleration = stats.Acceleration * dashControl;

                if (controlAcceleration > 0f)
                {
                    velocity = Vector2.MoveTowards(velocity, targetVelocity, controlAcceleration * deltaTime);
                }

                if (dashCooldown <= 0f && stats.DashImpulse > 0f)
                {
                    Vector2 dashVelocity = normalizedInput * stats.DashImpulse;
                    velocity = Vector2.Lerp(dashVelocity, velocity + dashVelocity, dashControl);
                    dashCooldown = dashInterval;
                }

                if (dashControl < 1f)
                {
                    float pulseDrag = stats.Drag * (1f - dashControl);
                    velocity = Vector2.MoveTowards(velocity, Vector2.zero, pulseDrag * deltaTime);
                }
            }
            else
            {
                velocity = Vector2.MoveTowards(velocity, Vector2.zero, stats.Drag * deltaTime);
            }

            velocity = Vector2.ClampMagnitude(velocity, stats.MaxSpeed + stats.DashImpulse);

            return velocity;
        }
    }
}
