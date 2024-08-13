using UnityEngine;

using ReupVirtualTwin.behaviourInterfaces;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.managers;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.webRequesters;

namespace ReupVirtualTwin.dependencyInjectors
{
    [RequireComponent(typeof(EditMediator))]
    public class EditMediatorDependecyInjector : MonoBehaviour
    {
        [SerializeField]
        GameObject insertPositionLocation;
        EditMediator editMediator;
        [SerializeField]
        GameObject editModeManager;
        [SerializeField]
        GameObject selectedObjectsManager;
        [SerializeField]
        GameObject transformObjectsManager;
        [SerializeField]
        GameObject deleteObjectsManager;
        [SerializeField]
        GameObject changeColorManager;
        [SerializeField]
        GameObject modelInfoManager;

        private void Awake()
        {
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
            if (webMessageSender == null )
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
                registry,
                editMediator
            );
            editMediator.originalSceneController = new OriginalSceneController(registry);
        }
    }
}
