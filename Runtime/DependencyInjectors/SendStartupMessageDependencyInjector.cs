using ReupVirtualTwin.behaviours;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.helpers;
using UnityEngine;

namespace ReupVirtualTwin.dependencyInjectors
{
    public class SendStartupMessageDependencyInjector : MonoBehaviour
    {
        private void Awake()
        {
            SendStartupMessage sendStartupMessage = GetComponent<SendStartupMessage>();
            IdController idGetterController = new IdController();
            TagsController tagsController = new TagsController();
            sendStartupMessage.objectMapper = new ObjectMapper(tagsController, idGetterController);
        }
    }
}
