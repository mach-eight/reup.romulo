using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.models
{
    public class ObjectTexturesRecord : IObjectTexturesRecord
    {
        public Dictionary<string, Texture> objectIdsToTexturesRecord;
        public Dictionary<Texture, HashSet<string>> texturesToObjectIdsRecord;

        public ObjectTexturesRecord()
        {
            objectIdsToTexturesRecord = new Dictionary<string, Texture>();
            texturesToObjectIdsRecord = new Dictionary<Texture, HashSet<string>>();
        }

        public void UpdateRecords(GameObject obj)
        {
            string objId = obj.GetInstanceID().ToString();
            Texture newTexture = obj.GetComponent<Renderer>().material.GetTexture("_BaseMap");
            Texture oldTexture = objectIdsToTexturesRecord.GetValueOrDefault(objId);
            UpdateTextureRecords(objId, oldTexture, newTexture);
            UpdateObjectIdsRecords(objId, newTexture);
        }

        public void CleanRecords(GameObject obj)
        {
            string objId = obj.GetInstanceID().ToString();
            Texture oldTexture = objectIdsToTexturesRecord.GetValueOrDefault(objId);
            RemoveObjectIdFromTextureRecord(objId, oldTexture);
            RemoveObjectIdRecord(objId);
            CheckTexturesIsStillBeingUsed(oldTexture);
        }

        private void UpdateTextureRecords(string objId, Texture oldTexture, Texture newTexture)
        {
            RemoveObjectIdFromTextureRecord(objId, oldTexture);
            AddObjectIdToTextureRecord(objId, newTexture);
            CheckTexturesIsStillBeingUsed(oldTexture);
        }
        private void UpdateObjectIdsRecords(string objId, Texture newTexture)
        {
            if (newTexture == null)
            {
                return;
            }
            objectIdsToTexturesRecord[objId] = newTexture;
        }

        private void AddObjectIdToTextureRecord(string objId, Texture newTexture)
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

        private void RemoveObjectIdFromTextureRecord(string objId, Texture oldTexture)
        {
            if (oldTexture != null && texturesToObjectIdsRecord[oldTexture] != null)
            {
                texturesToObjectIdsRecord[oldTexture].Remove(objId);
            }
        }

        private void RemoveObjectIdRecord(string objId)
        {
            objectIdsToTexturesRecord.Remove(objId);
        }

        private void CheckTexturesIsStillBeingUsed(Texture texture)
        {
            if (texture != null && texturesToObjectIdsRecord[texture].Count == 0)
            {
                texturesToObjectIdsRecord.Remove(texture);
                GameObject.Destroy(texture);
            }
        }
    }
}