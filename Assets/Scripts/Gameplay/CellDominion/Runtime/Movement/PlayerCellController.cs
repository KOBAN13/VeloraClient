using Gameplay.CellDominion.Runtime.Config;
using Gameplay.CellDominion.Runtime.Stats;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.CellDominion.Runtime.Movement
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PlayerCellController : MonoBehaviour
    {
        [SerializeField] private CellMovementConfig movementConfig;
        [SerializeField] private int level = 1;
        [SerializeField] private Camera worldCamera;
        [SerializeField] private float mouseDeadZone = 0.15f;

        private readonly CellBoostStack boostStack = new CellBoostStack();
        private readonly CellMovementMotor motor = new CellMovementMotor();
        private Rigidbody2D body;
        private Vector2 inputDirection;
        private CellMovementStats currentStats;

        public int Level
        {
            get
            {
                return level;
            }
        }

        public Vector2 AimDirection
        {
            get
            {
                return inputDirection;
            }
        }

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.freezeRotation = true;
            body.linearDamping = 0f;
            worldCamera = worldCamera == null ? Camera.main : worldCamera;
            RefreshStats();
        }

        private void Update()
        {
            inputDirection = ReadMovementInput();
        }

        private void FixedUpdate()
        {
            RefreshStats();
            Vector2 velocity = motor.Tick(inputDirection, currentStats, Time.fixedDeltaTime);
            body.linearVelocity = velocity;
            transform.localScale = Vector3.one * currentStats.Size;
        }

        public void SetMovementConfig(CellMovementConfig config)
        {
            movementConfig = config;
            RefreshStats();
        }

        public void SetLevel(int value)
        {
            level = Mathf.Max(1, value);
            RefreshStats();
        }

        public void AddBoost(CellStatModifier modifier)
        {
            boostStack.Add(modifier);
            RefreshStats();
        }

        public void ClearBoosts()
        {
            boostStack.Clear();
            RefreshStats();
        }

        private Vector2 ReadMovementInput()
        {
            Mouse mouse = Mouse.current;

            if (mouse == null)
            {
                return Vector2.zero;
            }

            if (worldCamera == null)
            {
                worldCamera = Camera.main;
            }

            if (worldCamera == null)
            {
                return Vector2.zero;
            }

            Vector2 mouseScreenPosition = mouse.position.ReadValue();
            Vector3 mouseWorldPosition = worldCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, -worldCamera.transform.position.z));
            Vector2 direction = (Vector2)mouseWorldPosition - body.position;

            if (direction.sqrMagnitude <= mouseDeadZone * mouseDeadZone)
            {
                return Vector2.zero;
            }

            return direction.normalized;
        }

        private void RefreshStats()
        {
            CellMovementStats baseStats = movementConfig == null
                ? new CellMovementStats(2.2f, 7f, 5f, 4.5f, 0.75f, 0f, 0.8f)
                : movementConfig.GetStats(level);

            currentStats = new CellMovementStats(
                boostStack.Evaluate(CellStatType.MaxSpeed, baseStats.MaxSpeed),
                boostStack.Evaluate(CellStatType.Acceleration, baseStats.Acceleration),
                boostStack.Evaluate(CellStatType.Drag, baseStats.Drag),
                boostStack.Evaluate(CellStatType.DashImpulse, baseStats.DashImpulse),
                boostStack.Evaluate(CellStatType.DashInterval, baseStats.DashInterval),
                boostStack.Evaluate(CellStatType.DashControl, baseStats.DashControl),
                boostStack.Evaluate(CellStatType.Size, baseStats.Size));
        }
    }
}
