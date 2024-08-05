using ReupVirtualTwin.behaviourInterfaces;
using ReupVirtualTwin.managers;
using ReupVirtualTwin.models;
using ReupVirtualTwin.behaviours;
using UnityEditor;
using UnityEngine;

public static class ReupSceneInstantiator
{
    static GameObject reupPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Assets/Quickstart/Reup.prefab");
    public class SceneObjects
    {
        public GameObject reupObject;
        public GameObject character;
        public GameObject baseGlobalScriptGameObject;
        public GameObject building;
        public ChangeColorManager changeColorManager;
        public SelectSelectableObject selectSelectableObject;
    }

    public static SceneObjects InstantiateScene()
    {
        GameObject reupGameObject = (GameObject)PrefabUtility.InstantiatePrefab(reupPrefab);
        GameObject baseGlobalScriptGameObject = reupGameObject.transform.Find("BaseGlobalScripts").gameObject;
        GameObject character = reupGameObject.transform.Find("Character").gameObject;

        GameObject building = new GameObject("building");
        building.AddComponent<RegisteredIdentifier>().AssignId("building-id");
        IBuildingGetterSetter setupBuilding = baseGlobalScriptGameObject.transform.Find("SetupBuilding").GetComponent<IBuildingGetterSetter>();
        setupBuilding.building = building;

        ChangeColorManager changeColorManager = baseGlobalScriptGameObject.transform
            .Find("EditMediator")
            .Find("ChangeColorManager")
            .GetComponent<ChangeColorManager>();

         SelectSelectableObject selectSelectableObject = baseGlobalScriptGameObject.transform
            .Find("EditMediator")
            .Find("SelectedObjectsManager")
            .GetComponent<SelectSelectableObject>();

        return new SceneObjects
        {
            reupObject = reupGameObject,
            character = character,
            baseGlobalScriptGameObject = baseGlobalScriptGameObject,
            building = building,
            changeColorManager = changeColorManager,
            selectSelectableObject = selectSelectableObject,
        };
    }

    public static void DestroySceneObjects(SceneObjects sceneObjects)
    {
        Object.Destroy(sceneObjects.reupObject);
        Object.Destroy(sceneObjects.building);
    }

    public static void SetEditMode(SceneObjects sceneObjects, bool editMode)
    {
        EditModeManager editModeManager = sceneObjects.baseGlobalScriptGameObject.transform
            .Find("EditMediator")
            .Find("EditModeManager")
            .GetComponent<EditModeManager>();
        editModeManager.editMode = editMode;
    }
}
