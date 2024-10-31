using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ReupVirtualTwin.editor
{
    public static class GUIUtils
    {
        public static void DrawSeparator()
        {
            DrawSeparator(new Color(0.5f, 0.5f, 0.5f));
        }
        public static void DrawSeparator(Color color, int thickness = 1, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }
    }

}
