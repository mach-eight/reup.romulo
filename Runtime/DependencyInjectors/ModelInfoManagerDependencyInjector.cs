using ReupVirtualTwin.managers;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.helpers;
using UnityEngine;
using ReupVirtualTwin.behaviourInterfaces;
using ReupVirtualTwin.modelInterfaces;

namespace ReupVirtualTwin.dependencyInjectors
{
    public class ModelInfoManagerDependencyInjector : MonoBehaviour
    {
        [SerializeField] GameObject setupBuilding;
        [SerializeField] GameObject spacesRecord;
        private void Awake()
        {
            ModelInfoManager sendModelInfoMessage = GetComponent<ModelInfoManager>();
            IdController idGetterController = new IdController();
            TagsController tagsController = new TagsController();
            sendModelInfoMessage.objectMapper = new ObjectMapper(tagsController, idGetterController);
            sendModelInfoMessage.setupBuilding = setupBuilding.GetComponent<IOnBuildingSetup>();
            sendModelInfoMessage.spacesRecord = spacesRecord.GetComponent<ISpacesRecord>();
        }
    }
}
