using UnityEngine;
using UnityEditor;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.dependencyInjectors;
using ReupVirtualTwin.webRequestersInterfaces;
using ReupVirtualTwin.webRequesters;
using System.Collections.Generic;

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
        ITagsApiConsumer tagsApiConsumer = new TagsApiConsumer();
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
                if (GUILayout.Button("Apply SketchUp tags to objects"))
                {
                    ApplyTagsToBuildingObjects(externalInstaller.building, tagsApiConsumer);
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

        public async void ApplyTagsToBuildingObjects(GameObject building, ITagsApiConsumer tagsApiConsumer)
        {
            List<string> errorMessages = await TagsApplierUtil.ApplyTags(building, tagsApiConsumer);
            if (errorMessages.Count > 0)
            {
                EditorUtility.DisplayDialog(
                    "Something went wrong while applying tags",
                    string.Join("\n", errorMessages),
                    "OK"
                );
            }
        }
    }
}
