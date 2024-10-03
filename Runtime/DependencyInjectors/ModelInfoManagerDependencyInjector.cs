using ReupVirtualTwin.managers;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.helpers;
using UnityEngine;
using ReupVirtualTwin.behaviourInterfaces;

namespace ReupVirtualTwin.dependencyInjectors
{
    public class ModelInfoManagerDependencyInjector : MonoBehaviour
    {
        [SerializeField]
        GameObject setupBuilding;
        private void Awake()
        {
            ModelInfoManager sendModelInfoMessage = GetComponent<ModelInfoManager>();
            IdController idGetterController = new IdController();
            TagsController tagsController = new TagsController();
            sendModelInfoMessage.objectMapper = new ObjectMapper(tagsController, idGetterController);
            sendModelInfoMessage.setupBuilding = setupBuilding.GetComponent<IOnBuildingSetup>();
        }
    }
}
