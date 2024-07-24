using ReupVirtualTwin.behaviours;
using ReupVirtualTwin.managers;
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
        public SetupBuilding setupbuilding;
    }

    public static SceneObjects InstantiateScene()
    {
        GameObject reupGameObject = (GameObject)PrefabUtility.InstantiatePrefab(reupPrefab);
        GameObject baseGlobalScriptGameObject = reupGameObject.transform.Find("BaseGlobalScripts").gameObject;
        GameObject character = reupGameObject.transform.Find("Character").gameObject;

        GameObject building = CreateBuilding();
        SetupBuilding setupBuilding = baseGlobalScriptGameObject.transform.Find("SetupBuilding").GetComponent<SetupBuilding>();
        setupBuilding.building = building;

        ChangeColorManager changeColorManager = baseGlobalScriptGameObject.transform
            .Find("EditionMediator")
            .Find("ChangeColorManager")
            .GetComponent<ChangeColorManager>();

        return new SceneObjects
        {
            reupObject = reupGameObject,
            character = character,
            baseGlobalScriptGameObject = baseGlobalScriptGameObject,
            building = building,
            changeColorManager = changeColorManager,
            setupbuilding = setupBuilding,
        };
    }

    public static void DestroySceneObjects(SceneObjects sceneObjects)
    {
        Object.Destroy(sceneObjects.reupObject);
        Object.Destroy(sceneObjects.building);
    }

    private static GameObject CreateBuilding()
    {
        GameObject building = new GameObject("building");
        GameObject child0 = new GameObject("child0");
        child0.transform.parent = building.transform;
        GameObject grandhChild0 = new GameObject("grandChild0");
        grandhChild0.transform.parent = child0.transform;
        return building;
    }
}
