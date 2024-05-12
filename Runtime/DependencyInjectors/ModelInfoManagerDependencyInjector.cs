using ReupVirtualTwin.managers;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.helpers;
using UnityEngine;

namespace ReupVirtualTwin.dependencyInjectors
{
    public class ModelInfoManagerDependencyInjector : MonoBehaviour
    {
        private void Awake()
        {
            ModelInfoManager sendModelInfoMessage = GetComponent<ModelInfoManager>();
            IdController idGetterController = new IdController();
            TagsController tagsController = new TagsController();
            sendModelInfoMessage.objectMapper = new ObjectMapper(tagsController, idGetterController);
        }
    }
}
