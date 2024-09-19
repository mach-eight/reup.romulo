using ReupVirtualTwin.managers;
using ReupVirtualTwin.behaviours;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace ReupVirtualTwinTests.utils
{
    public static class ReupSceneInstantiator
    {
        static GameObject reupPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Assets/Quickstart/Reup.prefab");
        public class SceneObjects
        {
            public GameObject reupObject;
            public Transform character;
            public Transform innerCharacter;
            public GameObject baseGlobalScriptGameObject;
            public GameObject building;
            public ChangeColorManager changeColorManager;
            public SetupBuilding setupbuilding;
            public SelectSelectableObject selectSelectableObject;
            public SelectedObjectsManager selectedObjectsManager;
            public EditMediator editMediator;
            public SensedObjectHighlighter selectableObjectHighlighter;
            public MoveDhvCamera moveDHVCamera;
            public GameObject dhvCamera;
            public GameObject fpvCamera;
            public ViewModeManager viewModeManager;
            public InputTestFixture input;
            public EventSystem eventSystem;
        }
        public static SceneObjects InstantiateSceneWithBuildingFromPrefab(GameObject buildingPrefab)
        {
            GameObject building = (GameObject)PrefabUtility.InstantiatePrefab(buildingPrefab);
            return SceneObjectsWithBuilding(building);
        }

        public static SceneObjects InstantiateScene()
        {
            GameObject building = CreateDefaultBuilding();
            return SceneObjectsWithBuilding(building);
        }

        private static SceneObjects SceneObjectsWithBuilding(GameObject building)
        {
            GameObject eventSystemGameObject = new GameObject("EventSystem");
            EventSystem eventSystem = eventSystemGameObject.AddComponent<EventSystem>();
            InputTestFixture input = new InputTestFixture();
            input.Setup();
            GameObject reupGameObject = (GameObject)PrefabUtility.InstantiatePrefab(reupPrefab);
            GameObject baseGlobalScriptGameObject = reupGameObject.transform.Find("BaseGlobalScripts").gameObject;
            Transform character = reupGameObject.transform.Find("Character");
            Transform innerCharacter = reupGameObject.transform.Find("Character").Find("InnerCharacter");


            SetupBuilding setupBuilding = baseGlobalScriptGameObject.transform.Find("SetupBuilding").GetComponent<SetupBuilding>();
            setupBuilding.building = building;

            EditMediator editMediator = baseGlobalScriptGameObject.transform
                .Find("EditMediator").GetComponent<EditMediator>();

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

            SensedObjectHighlighter selectableObjectHighlighter = baseGlobalScriptGameObject.transform
                .Find("HoverOverSelectablesObjects").GetComponent<SensedObjectHighlighter>();

            MoveDhvCamera moveDhvCamera = reupGameObject.transform
                .Find("DHVCinemachineCamera").GetComponent<MoveDhvCamera>();

            GameObject dhvCamera = reupGameObject.transform.Find("DHVCinemachineCamera").gameObject;
            GameObject fpvCamera = character.transform.Find("InnerCharacter").Find("FPVCinemachineCamera").gameObject;

            ViewModeManager viewModeManager = baseGlobalScriptGameObject.transform
                .Find("EditMediator")
                .Find("ViewModeManager").GetComponent<ViewModeManager>();


            return new SceneObjects
            {
                reupObject = reupGameObject,
                character = character,
                innerCharacter = innerCharacter,
                baseGlobalScriptGameObject = baseGlobalScriptGameObject,
                building = building,
                changeColorManager = changeColorManager,
                setupbuilding = setupBuilding,
                selectSelectableObject = selectSelectableObject,
                selectedObjectsManager = selectedObjectsManager,
                editMediator = editMediator,
                selectableObjectHighlighter = selectableObjectHighlighter,
                moveDHVCamera = moveDhvCamera,
                dhvCamera = dhvCamera,
                fpvCamera = fpvCamera,
                viewModeManager = viewModeManager,
                input = input,
                eventSystem = eventSystem,
            };
        }

        public static void DestroySceneObjects(SceneObjects sceneObjects)
        {
            Object.Destroy(sceneObjects.reupObject);
            Object.Destroy(sceneObjects.building);
            Object.Destroy(sceneObjects.eventSystem.gameObject);
            sceneObjects.input.TearDown();
        }

        private static GameObject CreateDefaultBuilding()
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

