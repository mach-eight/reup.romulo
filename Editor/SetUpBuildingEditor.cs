using UnityEngine;
using UnityEditor;
using ReupVirtualTwin.behaviourInterfaces;
using ReupVirtualTwin.behaviours;
using ReupVirtualTwin.controllers;

namespace ReupVirtualTwin.editor
{
    [CustomEditor(typeof(SetupBuilding))]
    public class SetUpBuildingEditor : Editor
    {
        bool showIdsOptions = false;
        bool showTagsOptions = false;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SetupBuilding setUpBuilding = (SetupBuilding)target;

            showIdsOptions = EditorGUILayout.Foldout(showIdsOptions, "Objects Ids");
            if (showIdsOptions)
            {
                if (setUpBuilding.idAssignerController == null)
                {
                    setUpBuilding.idAssignerController = new IdController();
                }
                if (GUILayout.Button("Add Ids to objects"))
                {
                    setUpBuilding.AssignIdsToBuilding();
                }
                if (GUILayout.Button("Remove Ids from objects"))
                {
                    setUpBuilding.RemoveIdsOfBuilding();
                }
                if (GUILayout.Button("Reset Ids from objects"))
                {
                    setUpBuilding.ResetIdsOfBuilding();
                }
            }
            showTagsOptions = EditorGUILayout.Foldout(showTagsOptions, "Objects Tags");
            if (showTagsOptions)
            {
                if (setUpBuilding.tagSystemController == null)
                {
                    setUpBuilding.tagSystemController = new TagSystemController();
                }
                if (GUILayout.Button("Add tag system to objects"))
                {
                    setUpBuilding.AddTagSystemToBuildingObjects();
                }
            }
        }
    }
}
