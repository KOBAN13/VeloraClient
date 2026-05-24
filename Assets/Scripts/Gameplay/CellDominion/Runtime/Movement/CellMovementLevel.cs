using System;

namespace Gameplay.CellDominion.Runtime.Movement
{
    [Serializable]
    public sealed class CellMovementLevel
    {
        public int Level = 1;
        public float MaxSpeed = 2.2f;
        public float Acceleration = 7f;
        public float Drag = 5f;
        public float DashImpulse = 4.5f;
        public float DashInterval = 0.75f;
        public float DashControl = 0.15f;
        public float Size = 0.8f;

        public CellMovementStats ToStats()
        {
            return new CellMovementStats(MaxSpeed, Acceleration, Drag, DashImpulse, DashInterval, DashControl, Size);
        }
    }
}
