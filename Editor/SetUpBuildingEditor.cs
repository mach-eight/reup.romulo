using UnityEngine;
using UnityEditor;
using ReupVirtualTwin.behaviours;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.controllerInterfaces;

namespace ReupVirtualTwin.editor
{
    [CustomEditor(typeof(SetupBuilding))]
    public class SetUpBuildingEditor : Editor
    {
        bool showIdsOptions = false;
        bool showTagsOptions = false;
        IIdAssignerController idAssignerController = new IdController();
        ITagSystemController tagSystemController = new TagSystemController();
        IObjectInfoController objectInfoController = new ObjectInfoController();
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            SetupBuilding setUpBuilding = (SetupBuilding)target;

            showIdsOptions = EditorGUILayout.Foldout(showIdsOptions, "Objects Ids");
            if (showIdsOptions)
            {
                if (GUILayout.Button("Add Ids to objects"))
                {
                    AssignIdsAndObjectInfoToBuilding(setUpBuilding.building);
                    Debug.Log("Ids and object info added to tree");
                }
                if (GUILayout.Button("Remove Ids from objects"))
                {
                    RemoveIdsAndObjectInfoFromBuilding(setUpBuilding.building);
                    Debug.Log("Ids and object info removed from tree");
                }
                if (GUILayout.Button("Reset Ids from objects"))
                {
                    ResetIdsOfBuilding(setUpBuilding.building);
                    Debug.Log("Ids and object info reseted from tree");
                }
            }
            showTagsOptions = EditorGUILayout.Foldout(showTagsOptions, "Objects Tags");
            if (showTagsOptions)
            {
                if (GUILayout.Button("Add tag system to objects"))
                {
                    AddTagSystemToBuildingObjects(setUpBuilding.building);
                    Debug.Log("tags script added to tree");
                }
            }
        }

        void AssignIdsAndObjectInfoToBuilding(GameObject building)
        {
            idAssignerController.AssignIdsToTree(building);
            objectInfoController.AssignObjectInfoToTree(building);
        }
        void RemoveIdsAndObjectInfoFromBuilding(GameObject building)
        {
            idAssignerController.RemoveIdsFromTree(building);
            objectInfoController.RemoveObjectInfoFromTree(building);
        }
        void ResetIdsOfBuilding(GameObject building)
        {
            RemoveIdsAndObjectInfoFromBuilding(building);
            AssignIdsAndObjectInfoToBuilding(building);
        }
        public void AddTagSystemToBuildingObjects(GameObject building)
        {
            tagSystemController.AssignTagSystemToTree(building);
        }
    }
}
