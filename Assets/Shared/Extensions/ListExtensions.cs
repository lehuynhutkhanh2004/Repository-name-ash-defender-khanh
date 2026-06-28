using System.Collections.Generic;
using UnityEngine;

namespace AshDefender.Shared.Extensions
{
    public static class ListExtensions
    {
        public static T GetRandom<T>(this IReadOnlyList<T> list) =>
            list[Random.Range(0, list.Count)];

        public static void Shuffle<T>(this List<T> list)
        {
            for (var i = list.Count - 1; i > 0; i--)
            {
                var j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        public static bool IsNullOrEmpty<T>(this IReadOnlyList<T> list) =>
            list == null || list.Count == 0;
    }
}
