using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ReupVirtualTwin.helpers;

namespace ReupVirtualTwin.editor
{
    public class ReupUtilsEditor : EditorWindow
    {
        public GameObject building;
        public GameObject tagsOriginObject;
        public GameObject tagsTargetObject;
        bool showActivateAllObjectsSection = false;
        bool showTransferTagsSection = false;

        [MenuItem("Reup Romulo/Reup utils")]
        public static void ShowWindow()
        {
            GetWindow<ReupUtilsEditor>("Reup utils");
        }

        void OnGUI()
        {
            showActivateAllObjectsSection = EditorGUILayout.Foldout(showActivateAllObjectsSection, "Activate all objects inside building");
            if (showActivateAllObjectsSection)
            {
                ActivateAllObjectsSection();
            }
            GUIUtils.DrawSeparator();
            showTransferTagsSection = EditorGUILayout.Foldout(showTransferTagsSection, "Transfer tags");
            if (showTransferTagsSection)
            {
                TransferTagsSection();
            }
        }
        void ActivateAllObjectsSection()
        {
            building = EditorGUILayout.ObjectField("Building", building, typeof(GameObject), true) as GameObject;
            if (GUILayout.Button("Activate all objects in Building"))
            {
                ObjectUtils.ActivateAllObjects(building);
            }
        }
        void TransferTagsSection()
        {
            tagsOriginObject = EditorGUILayout.ObjectField("Origin object", tagsOriginObject, typeof(GameObject), true) as GameObject;
            tagsTargetObject = EditorGUILayout.ObjectField("Target object", tagsTargetObject, typeof(GameObject), true) as GameObject;
            if (GUILayout.Button("Transfer tags"))
            {
                // ObjectUtils.TransferTags(tagsOriginObject, tagsTargetObject);
            }
        }

    }
}
