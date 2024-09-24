using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ReupVirtualTwin.editor
{
    public class ReupUtilsEditor : EditorWindow
    {
        public GameObject building;

        [MenuItem("Reup Romulo/Reup utils")]
        public static void ShowWindow()
        {
            GetWindow<ReupUtilsEditor>("Reup utils");
        }

        void OnGUI()
        {
            building = EditorGUILayout.ObjectField("Building", building, typeof(GameObject), true) as GameObject;
            if (GUILayout.Button("Activate all objects in Building"))
            {
                ActivateAllObjects(building);
            }
        }

        private void ActivateAllObjects(GameObject obj)
        {
            obj.SetActive(true);
            foreach (Transform child in obj.transform)
            {
                ActivateAllObjects(child.gameObject);
            }
        }
    }
}
