using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.modelInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.behaviours
{
    [RequireComponent(typeof(IMaterialChanger))]
    [RequireComponent(typeof(IWebRequestTexture))]
    public class ReceiveMaterialChangeRequests : MonoBehaviour
    {
        Texture2D texture;
        Material material;
        List<GameObject> objects;

        IRegistry registry;
        IWebRequestTexture _webRequestTexture;
        IMaterialChanger _materialChanger;

        public IWebRequestTexture webRequestTexture
        {
            set => _webRequestTexture = value;
        }
        public IMaterialChanger materialChanger
        {
            set => _materialChanger = value;
        }

        private void Start()
        {
            registry = ObjectFinder.FindObjectRegistry().GetComponent<IRegistry>();
            webRequestTexture = GetComponent<IWebRequestTexture>();
            materialChanger = GetComponent<IMaterialChanger>();
        }

        public IEnumerator ReceiveSetMaterialRequest(string serializedRequest)
        {
            MaterialChangeRequest request = JsonUtility.FromJson<MaterialChangeRequest>(serializedRequest);
            yield return StartCoroutine(LoadTextureFromUrl(request.textureUri));
            CreateMaterialWithTexture();
            FindObjects(request.objectsIds);
            _materialChanger.SetNewMaterialToObjects(objects, request.submeshIndexes, material);
        }

        private IEnumerator LoadTextureFromUrl (string url) {
            yield return _webRequestTexture.GetTexture(url,
                onSuccess: texture =>
                {
                    this.texture = texture;
                },
                onError: error =>
                {
                    Debug.LogError("Error downloading texture: " + error);
                });
        }
        void CreateMaterialWithTexture()
        {
            material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            material.SetTexture("_BaseMap", texture);
        }
        void FindObjects(string[] ids)
        {
            objects = registry.GetItemsWithGuids(ids);
        }
    }
}
