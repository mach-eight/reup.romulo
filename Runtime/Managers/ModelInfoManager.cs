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
using Zenject;


namespace ReupVirtualTwin.managers
{
    public class ModelInfoManager : MonoBehaviour, IModelInfoManager, ISceneStateManager
    {
        public string buildVersion { get => _buildVersion; }
        public IObjectMapper objectMapper { set => _objectMapper = value; }

        string _buildVersion = "2024-10-23"; // format: YYYY-MM-DD
        public IOnBuildingSetup setupBuilding { get; set; }
        IObjectMapper _objectMapper;

        public ISpacesRecord spacesRecord { get; set; }

        GameObject buildingObject;

        [Inject]
        public void Init(
            [Inject(Id = "building")] GameObject building)
        {
            buildingObject = building;
        }

        public JObject GetSceneState()
        {
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

        public WebMessage<JObject> ObtainUpdateBuildingMessage()
        {
            JObject messagePayload = ObtainUpdateBuildingMessagePayload();
            WebMessage<JObject> message = new()
            {
                type = WebMessageType.updateBuilding,
                payload = messagePayload,
            };
            return message;
        }

        public void InsertObjectToBuilding(GameObject obj)
        {
            obj.transform.SetParent(buildingObject.transform);
        }

        private JObject ObtainUpdateBuildingMessagePayload()
        {
            ObjectDTO buildingDTO = ObtainBuildingDTO();
            JObject updateBuildingMessage = new()
            {
                {"building", JObject.FromObject(buildingDTO)},
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
                {"spaceSelectors", spaceSelectors},
                {"building", JObject.FromObject(buildingDTO)},
            };
            return startupMessage;
        }

        private ObjectDTO ObtainBuildingDTO()
        {
            ObjectDTO buildingDTO = _objectMapper.MapObjectTree(buildingObject);
            return buildingDTO;
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
