using System.Collections.Generic;
using Gameplay.CellDominion.Runtime.Movement;
using UnityEngine;

namespace Gameplay.CellDominion.Runtime.Config
{
    [CreateAssetMenu(fileName = "CellMovementConfig", menuName = "Cell Dominion/Cell Movement Config")]
    public sealed class CellMovementConfig : ScriptableObject
    {
        [SerializeField] private List<CellMovementLevel> movementLevels = new List<CellMovementLevel>();

        public CellMovementStats GetStats(int level)
        {
            CellMovementLevelTable levelTable = new CellMovementLevelTable(movementLevels);

            return levelTable.GetStats(level);
        }
    }
}
