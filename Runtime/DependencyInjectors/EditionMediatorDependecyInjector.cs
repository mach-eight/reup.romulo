using UnityEngine;

using ReupVirtualTwin.behaviourInterfaces;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.managers;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.webRequesters;

namespace ReupVirtualTwin.dependencyInjectors
{
    [RequireComponent(typeof(EditionMediator))]
    public class EditionMediatorDependecyInjector : MonoBehaviour
    {
        EditionMediator _editionMediator;
        [SerializeField]
        GameObject editModeManager;
        [SerializeField]
        GameObject selectedObjectsManager;
        [SerializeField]
        GameObject transformObjectsManager;
        [SerializeField]
        GameObject insertObjectsManager;
        [SerializeField]
        GameObject deleteObjectsManager;

        private void Awake()
        {
            _editionMediator = GetComponent<EditionMediator>();
            ICharacterRotationManager _characterRotationManager = ObjectFinder.FindCharacter().GetComponent<ICharacterRotationManager>();
            _editionMediator.characterRotationManager = _characterRotationManager;
            _editionMediator.editModeManager = editModeManager.GetComponent<IEditModeManager>();
            _editionMediator.selectedObjectsManager = selectedObjectsManager.GetComponent<ISelectedObjectsManager>();
            _editionMediator.transformObjectsManager = transformObjectsManager.GetComponent<ITransformObjectsManager>();
            _editionMediator.deleteObjectsManager = deleteObjectsManager.GetComponent<IDeleteObjectsManager>();
            IWebMessagesSender webMessageSender = GetComponent<IWebMessagesSender>();
            if (webMessageSender == null )
            {
                throw new System.Exception("WebMessageSender not found to inject to edition mediator");
            }
            _editionMediator.webMessageSender = webMessageSender;
            _editionMediator.objectMapper = new ObjectMapper(new TagsController(), new IdController());
            //_editionMediator.insertObjectsManager = insertObjectsManager.GetComponent<IInsertObjectsManager>();
            _editionMediator.insertObjectsManager = new InserObjectController(_editionMediator, new MeshDownloader());
        }
    }
}
