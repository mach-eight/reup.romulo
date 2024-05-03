using ReupVirtualTwin.helpers;
using ReupVirtualTwin.behaviourInterfaces;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.enums;
using UnityEngine;
using ReupVirtualTwin.helperInterfaces;


namespace ReupVirtualTwin.behaviours
{
    [RequireComponent(typeof(IWebMessagesSender))]
    public class SendStartupMessage: MonoBehaviour, ISendStartupMessage
    {
        public string buildVersion { get => _buildVersion; }
        public IObjectMapper objectMapper { set => _objectMapper = value; }

        string _buildVersion = "2024-04-05";
        IWebMessagesSender webMessagesSender;
        IOnBuildingSetup setupBuilding;
        IObjectMapper _objectMapper;

        private void Awake()
        {
            LookForDependencySingletons();
        }

        private void LookForDependencySingletons()
        {
            setupBuilding = ObjectFinder.FindSetupBuilding()?.GetComponent<IOnBuildingSetup>();
            setupBuilding.onBuildingSetUp += SendMessage;
        }
        private void LookForDependencyComponents()
        {
            webMessagesSender = gameObject.GetComponent<IWebMessagesSender>();
        }

        public void SendMessage()
        {
            LookForDependencyComponents();
            WebMessage<StartupMessage> message = ObtainStartupMessage();
            webMessagesSender.SendWebMessage(message);
        }

        private WebMessage<StartupMessage> ObtainStartupMessage()
        {
            StartupMessage messagePayload = ObtainMessagePayload();
            WebMessage<StartupMessage> message = new()
            {
                type = WebMessageType.startupMessage,
                payload = messagePayload,
            };
            return message;
        }

        private StartupMessage ObtainMessagePayload()
        {
            ObjectDTO buildingDTO = ObtainBuildingDTO();
            StartupMessage startupMessage = new()
            {
                buildVersion = buildVersion,
                building = buildingDTO,
            };
            return startupMessage;
        }

        private ObjectDTO ObtainBuildingDTO()
        {
            GameObject buildingObject = ((IBuildingGetterSetter)setupBuilding).building;
            ObjectDTO buildingDTO = _objectMapper.MapObjectTree(buildingObject);
            return buildingDTO;
        }
    }
}
