using System;
using ReupVirtualTwin.managers;
using ReupVirtualTwin.behaviours;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using ReupVirtualTwin.models;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwinTests.mocks;

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
            public Transform dollhouseViewWrapper;
            public GameObject baseGlobalScriptGameObject;
            public GameObject building;
            public ChangeColorManager changeColorManager;
            public SetupBuilding setupBuilding;
            public SelectSelectableObject selectSelectableObject;
            public SelectedObjectsManager selectedObjectsManager;
            public EditMediator editMediator;
            public SensedObjectHighlighter selectableObjectHighlighter;
            public MoveDhvCamera moveDhvCameraBehavior;
            public GameObject dhvCamera;
            public GameObject fpvCamera;
            public ViewModeManager viewModeManager;
            public InputTestFixture input;
            public HeightMediator heightMediator;
            public ModelInfoManager modelInfoManager;
            public ObjectRegistry objectRegistry;
            public ObjectPool objectPool;
            public Camera mainCamera;
            public SpacesRecord spacesRecord;
            public WebMessageSenderSpy webMessageSenderSpy;
            public ITexturesManager texturesManager;
            public GesturesManager gesturesManager;
            public ZoomDhvCamera zoomDhvCameraBehavior;
            public GameObject houseContainer;
        }
        public static SceneObjects InstantiateSceneWithBuildingFromPrefab(GameObject buildingPrefab)
        {
            GameObject building = (GameObject)PrefabUtility.InstantiatePrefab(buildingPrefab);
            return InstantiateSceneWithBuildingObject(building);
        }
        public static SceneObjects InstantiateSceneWithBuildingFromPrefab(GameObject buildingPrefab, Action<GameObject> modifyBuilding)
        {
            GameObject building = (GameObject)PrefabUtility.InstantiatePrefab(buildingPrefab);
            modifyBuilding(building);
            return InstantiateSceneWithBuildingObject(building);
        }

        public static SceneObjects InstantiateScene()
        {
            GameObject building = CreateDefaultBuilding();
            return InstantiateSceneWithBuildingObject(building);
        }

        public static SceneObjects InstantiateSceneWithBuildingObject(GameObject building)
        {
            InputTestFixture input = new InputTestFixture();
            input.Setup();
            GameObject reupGameObject = (GameObject)PrefabUtility.InstantiatePrefab(reupPrefab);
            GameObject baseGlobalScriptGameObject = reupGameObject.transform.Find("BaseGlobalScripts").gameObject;
            Transform character = reupGameObject.transform.Find("Character");
            Transform innerCharacter = reupGameObject.transform.Find("Character").Find("InnerCharacter");
            Transform dollhouseViewWrapper = reupGameObject.transform.Find("DollhouseViewWrapper");

            SetupBuilding setupBuilding = baseGlobalScriptGameObject.transform.Find("SetupBuilding").GetComponent<SetupBuilding>();

            setupBuilding.building = building;
            setupBuilding.AssignIdsAndObjectInfoToBuilding();

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

            GameObject dhvCamera = reupGameObject.transform
                .Find("DollhouseViewWrapper")
                .Find("VerticalRotationWrapper")
                .Find("DHVCinemachineCamera").gameObject;

            GameObject fpvCamera = character.transform.Find("InnerCharacter").Find("FPVCinemachineCamera").gameObject;

            ViewModeManager viewModeManager = baseGlobalScriptGameObject.transform
                .Find("EditMediator")
                .Find("ViewModeManager").GetComponent<ViewModeManager>();

            HeightMediator heightMediator = character.transform.Find("Behaviours")
                .Find("HeightMediator").GetComponent<HeightMediator>();

            SpacesRecord spacesRecord = baseGlobalScriptGameObject.transform.Find("SpacesRecord").GetComponent<SpacesRecord>();

            MoveDhvCamera moveDhvCamera = dollhouseViewWrapper.GetComponent<MoveDhvCamera>();
            MoveDhvCamera moveDhvCameraBehavior = dollhouseViewWrapper.GetComponent<MoveDhvCamera>();

            ZoomDhvCamera zoomDhvCameraBehavior = dollhouseViewWrapper.GetComponent<ZoomDhvCamera>();

            ModelInfoManager modelInfoManager = baseGlobalScriptGameObject.transform.Find("ModelInfo").GetComponent<ModelInfoManager>();

            ObjectRegistry objectRegistry = baseGlobalScriptGameObject.transform.Find("ObjectRegistry").GetComponent<ObjectRegistry>();

            ObjectPool objectPool = baseGlobalScriptGameObject.transform.Find("ObjectPool").GetComponent<ObjectPool>();

            Camera mainCamera = reupGameObject.transform.Find("Main_Camera").GetComponent<Camera>();

            WebMessageSenderSpy webMessageSender = new WebMessageSenderSpy();
            editMediator.webMessageSender = webMessageSender;

            ITexturesManager texturesManager = baseGlobalScriptGameObject.transform.Find("TexturesManager").GetComponent<ITexturesManager>();

            GesturesManager gesturesManager = baseGlobalScriptGameObject.transform.Find("GesturesManager").GetComponent<GesturesManager>();

            GameObject houseContainer = reupGameObject.transform.Find("HouseContainer").gameObject;

            return new SceneObjects
            {
                reupObject = reupGameObject,
                character = character,
                innerCharacter = innerCharacter,
                dollhouseViewWrapper = dollhouseViewWrapper,
                baseGlobalScriptGameObject = baseGlobalScriptGameObject,
                building = building,
                changeColorManager = changeColorManager,
                setupBuilding = setupBuilding,
                selectSelectableObject = selectSelectableObject,
                selectedObjectsManager = selectedObjectsManager,
                editMediator = editMediator,
                selectableObjectHighlighter = selectableObjectHighlighter,
                dhvCamera = dhvCamera,
                fpvCamera = fpvCamera,
                viewModeManager = viewModeManager,
                input = input,
                heightMediator = heightMediator,
                moveDhvCameraBehavior = moveDhvCameraBehavior,
                modelInfoManager = modelInfoManager,
                objectRegistry = objectRegistry,
                objectPool = objectPool,
                mainCamera = mainCamera,
                spacesRecord = spacesRecord,
                webMessageSenderSpy = webMessageSender,
                texturesManager = texturesManager,
                gesturesManager = gesturesManager,
                zoomDhvCameraBehavior = zoomDhvCameraBehavior,
                houseContainer = houseContainer,
            };
        }

        public static void DestroySceneObjects(SceneObjects sceneObjects)
        {
            GameObject.Destroy(sceneObjects.reupObject);
            GameObject.Destroy(sceneObjects.building);
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

