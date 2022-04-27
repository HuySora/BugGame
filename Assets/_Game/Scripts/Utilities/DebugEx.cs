namespace BugGame
{
    using UnityEngine;

    public static class DebugEx {
        public const string kNullStr = "<b>[Null]</b>";

        public static void LogNull(object message, Object context = null) => Debug.LogWarning(kNullStr + " " + message, context);

        // Instance log
        public static void Log(this Object sender, object message, Object context = null) => Debug.Log($"<b>[{sender.name}]</b> " + message, context);
        public static void LogNull(this Object sender, object message, Object context = null) {
            string senderBlock = sender == null ? "NullSender" : sender.name;
            Debug.LogWarning($"<b>[{senderBlock}]</b>" + kNullStr + " " + message, context);
        }
    }
}

