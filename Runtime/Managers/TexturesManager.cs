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
            ApplyMaterial(obj, material);
            UpdateRecords(obj);
        }

        public void ApplyProtectedMaterialToObject(GameObject obj, Material material)
        {
            ApplyMaterial(obj, material);
            CleanAllObjectRecords(obj);
        }

        void ApplyMaterial(GameObject obj, Material material)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            renderer.material = material;
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
            CleanOldTextureRecordFromObject(objId, oldTexture);
            CreateNewTextureRecord(objId, newTexture);
            CheckTexturesIsStillBeingUsed(oldTexture);
        }

        void CreateNewTextureRecord(string objId, Texture newTexture)
        {
            if (newTexture == null)
            {
                return;
            }
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
            CleanOldTextureRecordFromObject(objId, oldTexture);
            CheckTexturesIsStillBeingUsed(oldTexture);
        }

        void CleanOldTextureRecordFromObject(string objId, Texture oldTexture)
        {
            if (oldTexture != null && texturesToObjectIdsRecord[oldTexture] != null)
            {
                texturesToObjectIdsRecord[oldTexture].Remove(objId);
            }
        }
        void CheckTexturesIsStillBeingUsed(Texture texture)
        {
            if (texture != null && texturesToObjectIdsRecord[texture].Count == 0)
            {
                texturesToObjectIdsRecord.Remove(texture);
                Destroy(texture);
            }
        }
    }

}