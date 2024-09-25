using System.Collections;
using System.Collections.Generic;
using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.managerInterfaces;
using UnityEngine;

namespace ReupVirtualTwin.managers
{
    public class TexturesManager : MonoBehaviour, ITexturesManager
    {
        public IIdGetterController idGetterController { set; get; }
        Dictionary<string, Texture> objectIdsToTexturesRecord;
        Dictionary<Texture, HashSet<string>> texturesToObjectIdsRecord;

        private void Awake()
        {
            objectIdsToTexturesRecord = new Dictionary<string, Texture>();
            texturesToObjectIdsRecord = new Dictionary<Texture, HashSet<string>>();
        }

        public void ApplyMaterialToObject(GameObject obj, Material material)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            renderer.material = material;
            Texture newTexture = material.GetTexture("_BaseMap");
            UpdateRecords(obj, newTexture);
        }

        void UpdateRecords(GameObject obj, Texture newTexture)
        {
            string objId = idGetterController.GetIdFromObject(obj);
            Texture oldTexture = objectIdsToTexturesRecord.GetValueOrDefault(objId);
            UpdateTextureRecords(objId, oldTexture, newTexture);
            objectIdsToTexturesRecord[objId] = newTexture;
        }
        void UpdateTextureRecords(string objId, Texture oldTexture, Texture newTexture)
        {
            if (oldTexture != null && texturesToObjectIdsRecord[oldTexture] != null)
            {
                texturesToObjectIdsRecord[oldTexture].Remove(objId);
                if (texturesToObjectIdsRecord[oldTexture].Count == 0)
                {
                    Destroy(oldTexture);
                    texturesToObjectIdsRecord.Remove(oldTexture);
                }
            }
            if (!texturesToObjectIdsRecord.ContainsKey(newTexture))
            {
                texturesToObjectIdsRecord[newTexture] = new HashSet<string>();
            }
            texturesToObjectIdsRecord[newTexture].Add(objId);
        }
    }

}