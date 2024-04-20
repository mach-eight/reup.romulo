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
    [RequireComponent(typeof(EditionMediator))]
    public class EditionMediatorDependecyInjector : MonoBehaviour
    {
        [SerializeField]
        GameObject insertPositionLocation;
        EditionMediator editionMediator;
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

        private void Awake()
        {
            editionMediator = GetComponent<EditionMediator>();
            ICharacterRotationManager characterRotationManager = ObjectFinder.FindCharacter().GetComponent<ICharacterRotationManager>();
            editionMediator.characterRotationManager = characterRotationManager;
            editionMediator.editModeManager = editModeManager.GetComponent<IEditModeManager>();
            editionMediator.selectedObjectsManager = selectedObjectsManager.GetComponent<ISelectedObjectsManager>();
            editionMediator.transformObjectsManager = transformObjectsManager.GetComponent<ITransformObjectsManager>();
            editionMediator.deleteObjectsManager = deleteObjectsManager.GetComponent<IDeleteObjectsManager>();
            editionMediator.changeColorManager = changeColorManager.GetComponent<IChangeColorManager>();
            IWebMessagesSender webMessageSender = GetComponent<IWebMessagesSender>();
            if (webMessageSender == null )
            {
                throw new System.Exception("WebMessageSender not found to inject to edition mediator");
            }
            editionMediator.webMessageSender = webMessageSender;
            editionMediator.objectMapper = new ObjectMapper(new TagsController(), new IdController());
            IObjectRegistry registry = ObjectFinder.FindObjectRegistry().GetComponent<IObjectRegistry>();
            editionMediator.registry = registry;
            editionMediator.insertObjectsController = new InsertObjectController(
                editionMediator,
                new MeshDownloader(),
                insertPositionLocation.transform.position
            );
            _editionMediator.changeMaterialController = new ChangeMaterialController(new TextureDownloader(), registry);
        }
    }
}
