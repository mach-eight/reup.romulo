using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using ReupVirtualTwin.editor;
using ReupVirtualTwin.behaviours;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.controllers;

public class AutoBuildEditor : MonoBehaviour
{
    private static IIdAssignerController idAssignerController = new IdController();
    private static IIdSearchRepeatedController idSearchRepeatedController = new IdController();

    [MenuItem("Reup Romulo/Build")]
    public static void Build()
    {
        string path = EditorUtility.OpenFolderPanel("Select Build Folder", "", "");
        if (string.IsNullOrEmpty(path))
        {
            EditorUtility.DisplayDialog("Error", "Invalid build folder path", "OK");
            return;
        }

        AddShaders();

        if (!AddUniqueIDs())
        {
            EditorUtility.DisplayDialog("Error", "Failed to add unique IDs", "OK");
            return;
        }

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = new[] { UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path },
            locationPathName = path,
            target = BuildTarget.WebGL
        };


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

    public static void AddShaders()
    {
        // add shaders
        AddShaderUtil.AddAlwaysIncludedShader("sHTiF/HandleShader");
        AddShaderUtil.AddAlwaysIncludedShader("sHTiF/AdvancedHandleShader");
        EditorUtility.DisplayDialog("Success", "Custom shaders added", "OK");
    }

    public static bool AddUniqueIDs()
    {
        // get the building object
        GameObject setupBuilding = ObjectFinder.FindSetupBuilding();
        if (setupBuilding == null)
        {
            return false;
        }
        SetupBuilding setupBuildingComponent = setupBuilding.GetComponent<SetupBuilding>();
        GameObject building = setupBuildingComponent.building;
        if (building == null)
        {
            return false;
        }
        // recreate ids
        idAssignerController.RemoveIdsFromTree(building);
        idAssignerController.AssignIdsToTree(building);
        // check for repeated ids
        bool repeatedIds = idSearchRepeatedController.SearchRepeatedIds(building);
        if (repeatedIds)
        {
            return false;
        }
        EditorUtility.DisplayDialog("Success", "Unique IDs added", "OK");
        return true;
    }
}
