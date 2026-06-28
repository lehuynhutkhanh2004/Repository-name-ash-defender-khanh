using UnityEngine;

namespace AshDefender.Shared.Utilities
{
    public static class MathUtility
    {
        public static int ClampMin(int value, int min) => Mathf.Max(value, min);

        public static float Percentage(int current, int max) => max > 0 ? (float)current / max : 0f;

        public static int ApplyPercentageBonus(int baseValue, float bonusPercent) =>
            Mathf.RoundToInt(baseValue * (1f + bonusPercent / 100f));

        public static int CalculateDamage(int attack, int defense) => Mathf.Max(0, attack - defense);

        public static bool RollChance(float chance) => Random.value <= chance;
    }
}
