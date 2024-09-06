using ReupVirtualTwin.managers;
using ReupVirtualTwin.models;
using ReupVirtualTwin.behaviours;
using UnityEditor;
using UnityEngine;

namespace ReupVirtualTwinTests.utils
{
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
            public SelectSelectableObject selectSelectableObject;
            public SelectedObjectsManager selectedObjectsManager;
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
                .Find("EditMediator")
                .Find("ChangeColorManager")
                .GetComponent<ChangeColorManager>();

            SelectSelectableObject selectSelectableObject = baseGlobalScriptGameObject.transform
               .Find("EditMediator")
               .Find("SelectedObjectsManager")
               .GetComponent<SelectSelectableObject>();

            SelectedObjectsManager selectedObjectsManager = baseGlobalScriptGameObject.transform
               .Find("EditMediator")
               .Find("SelectedObjectsManager")
               .GetComponent<SelectedObjectsManager>();

            return new SceneObjects
            {
                reupObject = reupGameObject,
                character = character,
                baseGlobalScriptGameObject = baseGlobalScriptGameObject,
                building = building,
                changeColorManager = changeColorManager,
                setupbuilding = setupBuilding,
                selectSelectableObject = selectSelectableObject,
                selectedObjectsManager = selectedObjectsManager,
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
        public static void SetEditMode(SceneObjects sceneObjects, bool editMode)
        {
            EditModeManager editModeManager = sceneObjects.baseGlobalScriptGameObject.transform
                .Find("EditMediator")
                .Find("EditModeManager")
                .GetComponent<EditModeManager>();
            editModeManager.editMode = editMode;
        }
    }
}

