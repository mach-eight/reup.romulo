using UnityEngine;

using ReupVirtualTwin.behaviourInterfaces;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.managers;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.webRequesters;
using ReupVirtualTwin.models;
using Zenject;

namespace ReupVirtualTwin.dependencyInjectors
{
    [RequireComponent(typeof(EditMediator))]
    public class EditMediatorDependecyInjector : MonoBehaviour
    {
        EditMediator editMediator;
        [SerializeField] GameObject insertPositionLocation;
        [SerializeField] EditModeManager editModeManager;
        [SerializeField] SelectedObjectsManager selectedObjectsManager;
        [SerializeField] TransformObjectsManager transformObjectsManager;
        [SerializeField] DeleteObjectsManager deleteObjectsManager;
        [SerializeField] ChangeColorManager changeColorManager;
        [SerializeField] ModelInfoManager modelInfoManager;
        [SerializeField] GameObject character;
        [SerializeField] GameObject fpvCamera;
        [SerializeField] GameObject dhvCamera;
        [SerializeField] ViewModeManager viewModeManager;
        [SerializeField] SpacesRecord spacesRecord;
        [SerializeField] TexturesManager texturesManager;
        [SerializeField] ObjectRegistry objectRegistry;
        GameObject building;

        [Inject]
        public void Init(
            [Inject(Id = "building")] GameObject building)
        {
            this.building = building;
        }

        private void Awake()
        {
            if (!insertPositionLocation ||
                !editModeManager ||
                !selectedObjectsManager ||
                !transformObjectsManager ||
                !deleteObjectsManager ||
                !changeColorManager ||
                !modelInfoManager ||
                !fpvCamera ||
                !dhvCamera ||
                !spacesRecord ||
                !texturesManager ||
                !character ||
                !viewModeManager)
            {
                throw new System.Exception("Some dependencies are missing");
            }
            editMediator = GetComponent<EditMediator>();
            editMediator.editModeManager = editModeManager;
            editMediator.selectedObjectsManager = selectedObjectsManager;
            editMediator.transformObjectsManager = transformObjectsManager;
            editMediator.deleteObjectsManager = deleteObjectsManager;
            editMediator.changeColorManager = changeColorManager;
            editMediator.modelInfoManager = modelInfoManager;
            IWebMessagesSender webMessageSender = GetComponent<IWebMessagesSender>();
            if (webMessageSender == null)
            {
                throw new System.Exception("WebMessageSender not found to inject to edit mediator");
            }
            editMediator.webMessageSender = webMessageSender;
            editMediator.objectMapper = new ObjectMapper(new TagsController(), new IdController());
            editMediator.registry = objectRegistry;
            editMediator.insertObjectsController = new InsertObjectController(
                editMediator,
                new MeshDownloader(),
                insertPositionLocation.transform.position,
                modelInfoManager.GetComponent<IModelInfoManager>()
            );
            editMediator.changeMaterialController = new ChangeMaterialController(
                new TextureDownloader(),
                objectRegistry,
                texturesManager
            );
            editMediator.originalSceneController = new OriginalSceneController(objectRegistry, texturesManager);
            editMediator.viewModeManager = viewModeManager;
            editMediator.spacesRecord = spacesRecord;
            editMediator.buildingVisibilityController = new BuildingVisibilityController(
                objectRegistry,
                building
            );
        }
    }
}
