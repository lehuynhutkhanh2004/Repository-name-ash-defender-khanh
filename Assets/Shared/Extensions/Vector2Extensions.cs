using UnityEngine;

namespace AshDefender.Shared.Extensions
{
    public static class Vector2Extensions
    {
        public static float DistanceTo(this Vector2 from, Vector2 to) => Vector2.Distance(from, to);

        public static bool IsWithinRange(this Vector2 from, Vector2 to, float range) =>
            Vector2.SqrMagnitude(to - from) <= range * range;

        public static Vector2 DirectionTo(this Vector2 from, Vector2 to) => (to - from).normalized;

        public static Vector2 WithX(this Vector2 v, float x) => new(x, v.y);

        public static Vector2 WithY(this Vector2 v, float y) => new(v.x, y);
    }
}
