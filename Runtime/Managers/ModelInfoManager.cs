using ReupVirtualTwin.helpers;
using ReupVirtualTwin.behaviourInterfaces;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.enums;
using UnityEngine;
using ReupVirtualTwin.helperInterfaces;
using ReupVirtualTwin.managerInterfaces;
using Newtonsoft.Json.Linq;
using ReupVirtualTwin.romuloEnvironment;
using ReupVirtualTwin.dataSchemas;


namespace ReupVirtualTwin.managers
{
    public class ModelInfoManager: MonoBehaviour, IModelInfoManager, ISceneStateManager
    {
        public string buildVersion { get => _buildVersion; }
        public IObjectMapper objectMapper { set => _objectMapper = value; }

        string _buildVersion = "2024-07-24";
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
        public JObject GetSceneState()
        {
            GameObject buildingObject = ObtainBuildingObject();
            JObject sceneState = _objectMapper.GetTreeSceneState(buildingObject);
            if (
                RomuloEnvironment.development &&
                !DataValidator.ValidateObjectToSchema(sceneState, RomuloInternalSchema.sceneStateSchema))
            {
                throw new System.Exception("Scene state does not match schema");
            }
            return sceneState;
        }

        public WebMessage<ModelInfoMessage> ObtainModelInfoMessage()
        {
            ModelInfoMessage messagePayload = ObtainModelInfoMessagePayload();
            WebMessage<ModelInfoMessage> message = new()
            {
                type = WebMessageType.requestModelInfoSuccess,
                payload = messagePayload,
            };
            return message;
        }

        public WebMessage<UpdateBuildingMessage> ObtainUpdateBuildingMessage()
        {
            UpdateBuildingMessage messagePayload = ObtainUpdateBuildingMessagePayload();
            WebMessage<UpdateBuildingMessage> message = new()
            {
                type = WebMessageType.updateBuilding,
                payload = messagePayload,
            };
            return message;
        }

        public void InsertObjectToBuilding(GameObject obj)
        {
            GameObject buildingObject = ObtainBuildingObject();
            obj.transform.SetParent(buildingObject.transform);
        }

        private UpdateBuildingMessage ObtainUpdateBuildingMessagePayload()
        {
            ObjectDTO buildingDTO = ObtainBuildingDTO();
            UpdateBuildingMessage updateBuildingMessage = new()
            {
                building = buildingDTO,
            };
            return updateBuildingMessage;
        }

        private ModelInfoMessage ObtainModelInfoMessagePayload()
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
            GameObject buildingObject = ObtainBuildingObject();
            ObjectDTO buildingDTO = _objectMapper.MapObjectTree(buildingObject);
            return buildingDTO;
        }

        private GameObject ObtainBuildingObject()
        {
            return ((IBuildingGetterSetter)setupBuilding).building;
        }

    }
}
