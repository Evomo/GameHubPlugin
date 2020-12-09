using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamehubPlugin.Core.Util {
    public static class DontDestroyOnLoadManager {
        static List<GameObject> _ddolObjects = new List<GameObject>();

        public static void DontDestroyOnLoad(this GameObject go) {
            UnityEngine.Object.DontDestroyOnLoad(go);
            _ddolObjects.Add(go);
        }

        public static void DestroyAll() {
            foreach (var go in _ddolObjects)
                if (go != null)
                {
                    UnityEngine.Object.Destroy(go);
                    Debug.Log($"Destroy {go.name}");
                }

            _ddolObjects.Clear();
        }
    }

    public static class Extensions {
        public static Color ContrastColor(Color color) {
            float h, s, v;
            Color.RGBToHSV(color, out h, out s, out v);
            return Color.HSVToRGB(h , s, 1 - v);
        }

        public static Color ToColor(this string color) {
            if (color.StartsWith("#", StringComparison.InvariantCulture)) {
                color = color.Substring(1); // strip #
            }

            if (color.Length == 6) {
                color += "FF"; // add alpha if missing
            }

            var hex = Convert.ToUInt32(color, 16);
            var r = ((hex & 0xff000000) >> 0x18) / 255f;
            var g = ((hex & 0xff0000) >> 0x10) / 255f;
            var b = ((hex & 0xff00) >> 8) / 255f;
            var a = ((hex & 0xff)) / 255f;

            return new Color(r, g, b, a);
        }
    }
}