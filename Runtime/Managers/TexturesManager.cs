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
            if (!ApplyMaterial(obj, material))
            {
                return;
            }
            UpdateRecords(obj);
        }

        public void ApplyProtectedMaterialToObject(GameObject obj, Material material)
        {
            if (!ApplyMaterial(obj, material))
            {
                return;
            }
            CleanAllObjectRecords(obj);
        }

        bool ApplyMaterial(GameObject obj, Material material)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer.material.GetTexture("_BaseMap") == material.GetTexture("_BaseMap"))
            {
                return false;
            }
            renderer.material = material;
            return true;
        }

        void UpdateRecords(GameObject obj)
        {
            Texture newTexture = obj.GetComponent<Renderer>().material.GetTexture("_BaseMap");
            string objId = idGetterController.GetIdFromObject(obj);
            Texture oldTexture = objectIdsToTexturesRecord.GetValueOrDefault(objId);
            UpdateTextureRecords(objId, oldTexture, newTexture);
            objectIdsToTexturesRecord[objId] = newTexture;
        }
        void UpdateTextureRecords(string objId, Texture oldTexture, Texture newTexture)
        {
            CleanOldTextureRecords(objId, oldTexture);
            CreateNewTextureRecord(objId, newTexture);
        }

        void CreateNewTextureRecord(string objId, Texture newTexture)
        {
            if (!texturesToObjectIdsRecord.ContainsKey(newTexture))
            {
                texturesToObjectIdsRecord[newTexture] = new HashSet<string>();
            }
            texturesToObjectIdsRecord[newTexture].Add(objId);
        }

        void CleanAllObjectRecords(GameObject obj)
        {
            string objId = idGetterController.GetIdFromObject(obj);
            Texture oldTexture = objectIdsToTexturesRecord.GetValueOrDefault(objId);
            CleanOldTextureRecords(objId, oldTexture);
        }

        void CleanOldTextureRecords(string objId, Texture oldTexture)
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
        }

    }

}