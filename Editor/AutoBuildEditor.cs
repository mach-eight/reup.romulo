using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using ReupVirtualTwin.editor;
using ReupVirtualTwin.behaviours;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.controllers;
using System.Collections.Generic;
using System.Linq;

public class AutoBuildEditor : MonoBehaviour
{
    private static IIdAssignerController idAssignerController = new IdController();
    private static IIdHasRepeatedController idHasRepeatedController = new IdController();
    private static IObjectInfoController objectInfoController = new ObjectInfoController();

    [MenuItem("Reup Romulo/Build")]
    public static void Build()
    {
        string path = EditorUtility.OpenFolderPanel("Select Build Folder", "", "");
        if (string.IsNullOrEmpty(path))
        {
            EditorUtility.DisplayDialog("Error", "Invalid build folder path", "OK");
            return;
        }

        if (Camera.allCamerasCount > 1)
        {
            EditorUtility.DisplayDialog(
                "Error", 
                "More than one camera found in the scene\n\n" +
                "Please erase any additional cameras outside the Reup Prefab", 
                "OK"
            );
            return;
        }

        GameObject building = GetBuilding();

        if (building == null)
        {
            EditorUtility.DisplayDialog("Error", "No building was found", "OK");
            return;
        }

        if (!CheckObjectsActiveStatus(building))
        {
            EditorUtility.DisplayDialog("Error", "Build canceled", "OK");
            return;
        }


        if (!AddUniqueIDs(building))
        {
            EditorUtility.DisplayDialog("Error", "Failed to add unique IDs", "OK");
            return;
        }

        AddShaders();

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = new[] { UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path },
            locationPathName = path,
            target = BuildTarget.WebGL
        };


        EditorUserBuildSettings.webGLBuildSubtarget = WebGLTextureSubtarget.ASTC;
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            EditorUtility.DisplayDialog("Success", "Build succeeded", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Error", "Build failed", "OK");
        }
        
    }

    private static void AddShaders()
    {
        AddShaderUtil.AddAlwaysIncludedShader("sHTiF/HandleShader");
        AddShaderUtil.AddAlwaysIncludedShader("sHTiF/AdvancedHandleShader");
        EditorUtility.DisplayDialog("Success", "Custom shaders configured", "OK");
    }

    private static bool AddUniqueIDs(GameObject buildingTree)
    {
        GameObject building = buildingTree;
        if (building == null)
        {
            return false;
        }

        ResetIdsAndAddObjectInfo(building);

        bool hasRepeatedIds = idHasRepeatedController.HasRepeatedIds(building);
        if (hasRepeatedIds)
        {
            Debug.LogError("Repeated IDs found");
            return false;
        }
        EditorUtility.DisplayDialog("Success", "Unique IDs added", "OK");
        return true;
    }

    private static GameObject GetBuilding()
    {
        GameObject setupBuilding = ObjectFinder.FindSetupBuilding();
        if (setupBuilding == null)
        {
            Debug.LogError("No setup building game object found");
            return null;
        }
        SetupBuilding setupBuildingComponent = setupBuilding.GetComponent<SetupBuilding>();
        GameObject building = setupBuildingComponent.building;
        if (building == null)
        {
            Debug.LogError("No building game object found");
            return null;
        }
        return building;
    }

    private static void ResetIdsAndAddObjectInfo(GameObject building)
    {
        idAssignerController.RemoveIdsFromTree(building);
        idAssignerController.AssignIdsToTree(building);
        objectInfoController.AssignObjectInfoToTree(building);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(building.scene);
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(building.scene);
    }

    private static bool CheckObjectsActiveStatus(GameObject buildingTree)
    {
        List<GameObject> disabledObjects = GetDisableObjects(buildingTree);
        if (disabledObjects.Count > 0)
        {
            string message = CreateDisableObjectsMessage(disabledObjects);
            bool continueBuild = EditorUtility.DisplayDialog(
                "Disabled Objects Found",
                message,
                "Continue",
                "Cancel"
            );

            if (!continueBuild)
            {
                return false;
            }
        }
        return true;
    }

    private static List<GameObject> GetDisableObjects(GameObject obj)
    {
        List<GameObject> disabledObjects = new List<GameObject>();
        bool isObjectActive = obj.activeSelf;
        if (!isObjectActive)
        {
            disabledObjects.Add(obj);
            return disabledObjects;
        }
        foreach (Transform child in obj.transform)
        {
            disabledObjects.AddRange(GetDisableObjects(child.gameObject));
        }

        return disabledObjects;
    }

    private static string CreateDisableObjectsMessage(List<GameObject> disabledObjects)
    {
        var firstTenDisabledObjects = disabledObjects.Take(10).Select(obj => obj.name);
        string disableObjectsNames = string.Join("\n", firstTenDisabledObjects);
        int totalDisabledObjectsCount = disabledObjects.Count;
        return $"Total number of disabled objects: {totalDisabledObjectsCount}\n\n" +
               $"Here is a list of the first 10 disabled objects:\n{disableObjectsNames}\n\n" +
               "This could damage the correct behavior of the model.\n" +
               "Do you want to continue?";
    }
}
