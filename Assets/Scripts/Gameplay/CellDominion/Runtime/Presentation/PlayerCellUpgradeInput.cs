using Gameplay.CellDominion.Runtime.Movement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.CellDominion.Runtime.Presentation
{
    public sealed class PlayerCellUpgradeInput : MonoBehaviour
    {
        [SerializeField] private PlayerCellController target;
        [SerializeField] private int maxLevel = 10;

        private void Awake()
        {
            if (target == null)
            {
                target = GetComponent<PlayerCellController>();
            }
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;

            if (keyboard == null || target == null)
            {
                return;
            }

            if (keyboard.pageUpKey.wasPressedThisFrame || keyboard.equalsKey.wasPressedThisFrame)
            {
                target.SetLevel(Mathf.Min(maxLevel, target.Level + 1));
            }

            if (keyboard.pageDownKey.wasPressedThisFrame || keyboard.minusKey.wasPressedThisFrame)
            {
                target.SetLevel(Mathf.Max(1, target.Level - 1));
            }
        }
    }
}
