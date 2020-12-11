using UnityEngine;

namespace GamehubPlugin.Core.Util {
    public static class GhHelpers{

        public static void Log(string message) {
            Debug.Log($"Unity-GameHub: {message}");
        }
    }
}