using UnityEditor;
using UnityEngine;

namespace Stensel.Editor {
    public static class Utils {
        public static float lineHeight = EditorGUIUtility.singleLineHeight + 2;
        
        public static Texture GetIcon(string icon) {
            var prefix = EditorGUIUtility.isProSkin ? "d_" : "";
            return EditorGUIUtility.IconContent(prefix + icon).image;
        }
    }
}