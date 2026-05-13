namespace Gameplay.CellDominion.Runtime.Movement
{
    public readonly struct CellMovementStats
    {
        public CellMovementStats(
            float maxSpeed,
            float acceleration,
            float drag,
            float dashImpulse,
            float dashInterval,
            float dashControl,
            float size)
        {
            MaxSpeed = maxSpeed;
            Acceleration = acceleration;
            Drag = drag;
            DashImpulse = dashImpulse;
            DashInterval = dashInterval;
            DashControl = dashControl;
            Size = size;
        }

        public float MaxSpeed { get; }

        public float Acceleration { get; }

        public float Drag { get; }

        public float DashImpulse { get; }

        public float DashInterval { get; }

        public float DashControl { get; }

        public float Size { get; }
    }
}
