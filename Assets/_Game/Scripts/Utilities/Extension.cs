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

        public static float GetOrthographicHeight(this Camera camera) => camera.orthographicSize * 2;
        public static float GetOrthographicWidth(this Camera camera) => camera.GetOrthographicHeight() * camera.aspect;

    }
}

