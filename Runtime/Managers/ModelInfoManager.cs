using ReupVirtualTwin.helpers;
using ReupVirtualTwin.behaviourInterfaces;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.enums;
using UnityEngine;
using ReupVirtualTwin.helperInterfaces;
using ReupVirtualTwin.managerInterfaces;


namespace ReupVirtualTwin.managers
{
    public class ModelInfoManager: MonoBehaviour, IModelInfoManager
    {
        public string buildVersion { get => _buildVersion; }
        public IObjectMapper objectMapper { set => _objectMapper = value; }

        string _buildVersion = "2024-04-05";
        IOnBuildingSetup setupBuilding;
        IObjectMapper _objectMapper;

        private void Awake()
        {
            LookForDependencySingletons();
        }

        private void LookForDependencySingletons()
        {
            setupBuilding = ObjectFinder.FindSetupBuilding()?.GetComponent<IOnBuildingSetup>();
        }

        public WebMessage<ModelInfoMessage> ObtainModelInfoMessage()
        {
            ModelInfoMessage messagePayload = ObtainMessagePayload();
            WebMessage<ModelInfoMessage> message = new()
            {
                type = WebMessageType.requestModelInfoSuccess,
                payload = messagePayload,
            };
            return message;
        }

        private ModelInfoMessage ObtainMessagePayload()
        {
            ObjectDTO buildingDTO = ObtainBuildingDTO();
            ModelInfoMessage startupMessage = new()
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
