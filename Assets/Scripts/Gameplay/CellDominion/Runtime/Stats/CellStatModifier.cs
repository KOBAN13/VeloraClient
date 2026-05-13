using System;

namespace Gameplay.CellDominion.Runtime.Stats
{
    [Serializable]
    public readonly struct CellStatModifier
    {
        public CellStatModifier(CellStatType type, CellStatModifierMode mode, float value)
        {
            Type = type;
            Mode = mode;
            Value = value;
        }

        public CellStatType Type { get; }

        public CellStatModifierMode Mode { get; }

        public float Value { get; }
    }
}
