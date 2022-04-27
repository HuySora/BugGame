namespace BugGame
{
    using UnityEngine;

    public static class Extension
    {
        public static bool TryNullCheckAndLog<T, TTrace, TSender>(this T target, string message, TTrace trace, TSender sender)
        where T : Object
        where TTrace : Object
        where TSender: Object
        {
            if (target == null && trace == null)
            {
                sender.LogNull($"Parameters are null with this message: {message}");
                return true;
            }
            if (target == null)
            {
                sender.LogNull($"{message} (click to trace {trace.name})", trace);
                return true;
            }

            return false;
        }

        public static bool IsInBound<T>(this T[,] map, Vector2Int first)
        {
            if (first.x < 0 || first.x >= map.GetLength(0)
            || first.y < 0 || first.y >= map.GetLength(1)) return false;

            return true;
        }
        
        public static float GetOrthographicHeight(this Camera camera) => camera.orthographicSize * 2;
        public static float GetOrthographicWidth(this Camera camera) => camera.GetOrthographicHeight() * camera.aspect;
        public static float ToAngle2D(this Vector2 dir, bool asRad = false)
        {
            float rad = Mathf.Atan2(dir.y, dir.x);

            return asRad ? rad : rad * Mathf.Rad2Deg;
        }
    }
}

