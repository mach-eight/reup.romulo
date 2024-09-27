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
using Newtonsoft.Json.Schema;
using ReupVirtualTwin.modelInterfaces;
using System.Linq;


namespace ReupVirtualTwin.managers
{
    public class ModelInfoManager : MonoBehaviour, IModelInfoManager, ISceneStateManager
    {
        public string buildVersion { get => _buildVersion; }
        public IObjectMapper objectMapper { set => _objectMapper = value; }

        string _buildVersion = "2024-09-24"; // format: YYYY-MM-DD
        public IOnBuildingSetup setupBuilding { get; set; }
        IObjectMapper _objectMapper;

        public ISpacesRecord spacesRecord { get; set; }

        public JObject GetSceneState()
        {
            GameObject buildingObject = ObtainBuildingObject();
            JObject sceneState = _objectMapper.GetTreeSceneState(buildingObject);
            if (
                RomuloEnvironment.development &&
                !sceneState.IsValid(RomuloInternalSchema.sceneStateSchema))
            {
                throw new System.Exception("Scene state does not match schema");
            }
            return sceneState;
        }

        public WebMessage<JObject> ObtainModelInfoMessage()
        {
            JObject messagePayload = ObtainModelInfoMessagePayload();
            WebMessage<JObject> message = new()
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

        private JObject ObtainModelInfoMessagePayload()
        {
            ObjectDTO buildingDTO = ObtainBuildingDTO();
            JArray spaceSelectors = ObtainSpaceSelectorsList();
            JObject startupMessage = new()
            {
                {"buildVersion", buildVersion},
                {"building", JObject.FromObject(buildingDTO)},
                {"spaceSelectors", spaceSelectors}
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

        JArray ObtainSpaceSelectorsList()
        {
            return new JArray(
                spacesRecord.jumpPoints.Select(jumpPoint => new JObject{
                    {"name", jumpPoint.spaceName},
                    {"id", jumpPoint.id},
                })
            );
        }

    }
}
