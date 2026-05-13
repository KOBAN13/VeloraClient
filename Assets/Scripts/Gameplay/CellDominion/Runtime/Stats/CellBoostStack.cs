using System.Collections.Generic;

namespace Gameplay.CellDominion.Runtime.Stats
{
    public sealed class CellBoostStack
    {
        private readonly List<CellStatModifier> modifiers;

        public CellBoostStack()
        {
            modifiers = new List<CellStatModifier>();
        }

        public void Add(CellStatModifier modifier)
        {
            modifiers.Add(modifier);
        }

        public void Clear()
        {
            modifiers.Clear();
        }

        public float Evaluate(CellStatType type, float baseValue)
        {
            float additiveValue = baseValue;
            float multiplier = 1f;

            for (int i = 0; i < modifiers.Count; i++)
            {
                CellStatModifier modifier = modifiers[i];

                if (modifier.Type != type)
                {
                    continue;
                }

                if (modifier.Mode == CellStatModifierMode.Add)
                {
                    additiveValue += modifier.Value;
                }
                else
                {
                    multiplier *= modifier.Value;
                }
            }

            return additiveValue * multiplier;
        }
    }
}
