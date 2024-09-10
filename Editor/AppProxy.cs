using UnityEngine;
using UnityEditor;


namespace ReupVirtualTwin.editor
{
    public class AppProxy : EditorWindow
    {
        bool viewControls = false;
        bool dollhouseView;
        bool firstPersonView;
        public string[] selStrings = new string[] {"Grid 1", "Grid 2", "Grid 3", "Grid 4"};


        [MenuItem("Reup Romulo/App proxy")]

        public static void ShowWindow()
        {
            GetWindow<AppProxy>("App proxy");
        }
        void OnGUI()
        {
            viewControls = EditorGUILayout.Foldout(viewControls, "View controls");
            if (viewControls)
            {
                ViewControls();
            }
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }
        void ViewControls()
        {
            dollhouseView = EditorGUILayout.Toggle("Dollhouse view", dollhouseView);
            firstPersonView = EditorGUILayout.Toggle("First-person view", firstPersonView);
        }
    }
}
