using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.CellDominion.Runtime.Movement
{
    public sealed class CellMovementLevelTable
    {
        private readonly IReadOnlyList<CellMovementLevel> levels;

        public CellMovementLevelTable(IReadOnlyList<CellMovementLevel> levels)
        {
            this.levels = levels;
        }

        public CellMovementStats GetStats(int level)
        {
            if (levels == null || levels.Count == 0)
            {
                return new CellMovementStats(2.2f, 7f, 5f, 4.5f, 0.75f, 0.15f, 0.8f);
            }

            CellMovementLevel previousLevel = levels[0];

            if (level <= previousLevel.Level)
            {
                return previousLevel.ToStats();
            }

            for (int i = 1; i < levels.Count; i++)
            {
                CellMovementLevel nextLevel = levels[i];

                if (level <= nextLevel.Level)
                {
                    float levelRange = nextLevel.Level - previousLevel.Level;
                    float progress = levelRange <= 0f ? 1f : (level - previousLevel.Level) / levelRange;

                    return Lerp(previousLevel, nextLevel, progress);
                }

                previousLevel = nextLevel;
            }

            return previousLevel.ToStats();
        }

        private CellMovementStats Lerp(CellMovementLevel from, CellMovementLevel to, float progress)
        {
            float normalizedProgress = Mathf.Clamp01(progress);

            return new CellMovementStats(
                Mathf.Lerp(from.MaxSpeed, to.MaxSpeed, normalizedProgress),
                Mathf.Lerp(from.Acceleration, to.Acceleration, normalizedProgress),
                Mathf.Lerp(from.Drag, to.Drag, normalizedProgress),
                Mathf.Lerp(from.DashImpulse, to.DashImpulse, normalizedProgress),
                Mathf.Lerp(from.DashInterval, to.DashInterval, normalizedProgress),
                Mathf.Lerp(from.DashControl, to.DashControl, normalizedProgress),
                Mathf.Lerp(from.Size, to.Size, normalizedProgress));
        }
    }
}
