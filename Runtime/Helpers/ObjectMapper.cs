using UnityEngine;
using System.Collections.Generic;

using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.helperInterfaces;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.controllerInterfaces;
using Newtonsoft.Json.Linq;

namespace ReupVirtualTwin.helpers
{
    public class ObjectMapper : IObjectMapper
    {
        ITagsController _tagsController;
        IIdGetterController _idGetterController;
        public ObjectMapper(ITagsController tagsController, IIdGetterController idGetterController)
        {
            _tagsController = tagsController;
            _idGetterController = idGetterController;
        }
        public ObjectDTO[] MapObjectsToDTO(List<GameObject> objs)
        {
            List<ObjectDTO> selectedDTOObjects = new List<ObjectDTO>();
            foreach (GameObject obj in objs)
            {
                selectedDTOObjects.Add(MapObjectToDTO(obj));
            }
            ObjectDTO[] objectDTOs = selectedDTOObjects.ToArray();
            return objectDTOs;
        }

        public ObjectDTO MapObjectToDTO(GameObject obj)
        {
            return new ObjectDTO
            {
                id = _idGetterController.GetIdFromObject(obj),
                tags = _tagsController.GetTagsFromObject(obj).ToArray(),
            };
        }

        public ObjectDTO MapObjectTree(GameObject obj)
        {
            ObjectDTO objectDTO = MapObjectToDTO(obj);
            objectDTO.children = new ObjectDTO[obj.transform.childCount];
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                objectDTO.children[i] = MapObjectTree(obj.transform.GetChild(i).gameObject);
            }
            return objectDTO;
        }

        public JObject GetTreeSceneState(GameObject obj)
        {
            JObject parentSceneState = GetObjectSceneState(obj);
            JArray children = new JArray();
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                children.Add(GetTreeSceneState(obj.transform.GetChild(i).gameObject));
            }
            parentSceneState.Add("children", children);
            return parentSceneState;
        }

        public JObject GetObjectSceneState(GameObject obj)
        {
            string objectId = _idGetterController.GetIdFromObject(obj);
            JObject objectMetaData = ObjectMetaDataUtils.GetMetaData(obj);
            if (objectMetaData == null)
            {
                return new JObject
                {
                    { "id", objectId },
                };
            }
            return new JObject
            {
                { "id", objectId },
                { "appearance", objectMetaData["appearance"] }
            };
        }
    }
}
