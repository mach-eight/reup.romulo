using UnityEngine;

using ReupVirtualTwin.behaviourInterfaces;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.managers;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.webRequesters;
using ReupVirtualTwin.models;

namespace ReupVirtualTwin.dependencyInjectors
{
    [RequireComponent(typeof(EditMediator))]
    public class EditMediatorDependecyInjector : MonoBehaviour
    {
        EditMediator editMediator;
        [SerializeField] GameObject insertPositionLocation;
        [SerializeField] GameObject editModeManager;
        [SerializeField] GameObject selectedObjectsManager;
        [SerializeField] GameObject transformObjectsManager;
        [SerializeField] GameObject deleteObjectsManager;
        [SerializeField] GameObject changeColorManager;
        [SerializeField] GameObject modelInfoManager;
        [SerializeField] GameObject character;
        [SerializeField] GameObject fpvCamera;
        [SerializeField] GameObject dhvCamera;
        [SerializeField] ViewModeManager viewModeManager;
        [SerializeField] SpacesRecord spacesRecord;

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
                !character)
            {
                throw new System.Exception("Some dependencies are missing");
            }
            editMediator = GetComponent<EditMediator>();
            ICharacterRotationManager characterRotationManager = ObjectFinder.FindCharacter().GetComponent<ICharacterRotationManager>();
            editMediator.characterRotationManager = characterRotationManager;
            editMediator.editModeManager = editModeManager.GetComponent<IEditModeManager>();
            editMediator.selectedObjectsManager = selectedObjectsManager.GetComponent<ISelectedObjectsManager>();
            editMediator.transformObjectsManager = transformObjectsManager.GetComponent<ITransformObjectsManager>();
            editMediator.deleteObjectsManager = deleteObjectsManager.GetComponent<IDeleteObjectsManager>();
            editMediator.changeColorManager = changeColorManager.GetComponent<IChangeColorManager>();
            editMediator.modelInfoManager = modelInfoManager.GetComponent<IModelInfoManager>();
            IWebMessagesSender webMessageSender = GetComponent<IWebMessagesSender>();
            if (webMessageSender == null)
            {
                throw new System.Exception("WebMessageSender not found to inject to edit mediator");
            }
            editMediator.webMessageSender = webMessageSender;
            editMediator.objectMapper = new ObjectMapper(new TagsController(), new IdController());
            IObjectRegistry registry = ObjectFinder.FindObjectRegistry().GetComponent<IObjectRegistry>();
            editMediator.registry = registry;
            editMediator.insertObjectsController = new InsertObjectController(
                editMediator,
                new MeshDownloader(),
                insertPositionLocation.transform.position,
                modelInfoManager.GetComponent<IModelInfoManager>()
            );
            editMediator.changeMaterialController = new ChangeMaterialController(
                new TextureDownloader(),
                registry
            );
            editMediator.originalSceneController = new OriginalSceneController(registry);
            editMediator.viewModeManager = viewModeManager;
            editMediator.spacesRecord = spacesRecord;
        }
    }
}
