using UnityEngine;
using UnityEditor;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.dependencyInjectors;

namespace ReupVirtualTwin.editor
{
    [CustomEditor(typeof(ExternalInstaller))]
    public class ExternalInstallerEditor : Editor
    {
        bool showIdsOptions = false;
        bool showTagsOptions = false;
        IIdAssignerController idAssignerController = new IdController();
        ITagSystemController tagSystemController = new TagSystemController();
        IObjectInfoController objectInfoController = new ObjectInfoController();
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ExternalInstaller externalInstaller = (ExternalInstaller)target;

            showIdsOptions = EditorGUILayout.Foldout(showIdsOptions, "Objects Ids");
            if (showIdsOptions)
            {
                if (GUILayout.Button("Add Ids to objects"))
                {
                    AssignIdsAndObjectInfoToBuilding(externalInstaller.building);
                    Debug.Log("Ids and object info added to tree");
                }
                if (GUILayout.Button("Remove Ids from objects"))
                {
                    RemoveIdsAndObjectInfoFromBuilding(externalInstaller.building);
                    Debug.Log("Ids and object info removed from tree");
                }
                if (GUILayout.Button("Reset Ids from objects"))
                {
                    ResetIdsOfBuilding(externalInstaller.building);
                    Debug.Log("Ids and object info reseted from tree");
                }
            }
            showTagsOptions = EditorGUILayout.Foldout(showTagsOptions, "Objects Tags");
            if (showTagsOptions)
            {
                if (GUILayout.Button("Add tag system to objects"))
                {
                    AddTagSystemToBuildingObjects(externalInstaller.building);
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
