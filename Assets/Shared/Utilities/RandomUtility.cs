using System.Collections.Generic;
using UnityEngine;

namespace AshDefender.Shared.Utilities
{
    public static class RandomUtility
    {
        public static T PickRandom<T>(IReadOnlyList<T> items) =>
            items[Random.Range(0, items.Count)];

        public static T PickWeighted<T>(IReadOnlyList<T> items, IReadOnlyList<float> weights)
        {
            var total = 0f;
            foreach (var w in weights) total += w;

            var roll = Random.value * total;
            var cumulative = 0f;
            for (var i = 0; i < items.Count; i++)
            {
                cumulative += weights[i];
                if (roll <= cumulative) return items[i];
            }
            return items[items.Count - 1];
        }

        public static int Range(int min, int maxExclusive) => Random.Range(min, maxExclusive);

        public static float Range(float min, float max) => Random.Range(min, max);
    }
}
