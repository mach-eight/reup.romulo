using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.webRequestersInterfaces;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.helpers;
using System;
using ReupVirtualTwin.romuloEnvironment;
using ReupVirtualTwin.dataSchemas;
using Newtonsoft.Json.Linq;

namespace ReupVirtualTwin.controllers
{
    public class ChangeMaterialController : IChangeMaterialController
    {
        readonly ITextureDownloader textureDownloader;
        readonly IObjectRegistry objectRegistry;
        readonly IMediator mediator;
        public ChangeMaterialController(ITextureDownloader textureDownloader, IObjectRegistry objectRegistry, IMediator mediator)
        {
            this.textureDownloader = textureDownloader;
            this.objectRegistry = objectRegistry;
            this.mediator = mediator;
        }

        public async Task ChangeObjectMaterial(JObject message)
        {
            Debug.Log(message.GetType());
            if (RomuloEnvironment.development)
            {
                Debug.Log("validating");
                if (!DataValidator.ValidateObjectToSchema(message, InternalSchema.materialChangeInfo))
                {
                    Debug.Log("paila");
                    return;
                }
                    Debug.Log("all good");
            }
            string materialUrl = message["material_url"].ToString();
            Debug.Log("materialUrl");
            Debug.Log(materialUrl);
            string[] objectIds = message["object_ids"].ToObject<string[]>();
            Debug.Log("objectIds");
            Debug.Log(objectIds);
            Debug.Log(objectIds.GetType());
            for (int i = 0; i < objectIds.Length; i++)
            {
                Debug.Log(objectIds[i]);
            }
            Texture2D texture = await textureDownloader.DownloadTextureFromUrl(materialUrl);
            if (!texture)
            {
                Debug.Log("there is no texture");
                mediator.Notify(ReupEvent.error, $"Error downloading image from {materialUrl}");
                return;
            }
            Debug.Log("3");
            List<GameObject> objects = objectRegistry.GetObjectsWithGuids(objectIds);
            Debug.Log("4");
            Material newMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            Debug.Log("5");
            newMaterial.SetTexture("_BaseMap", texture);
            Debug.Log("6");
            for (int i = 0; i < objects.Count; i++)
            {
            Debug.Log("7");
                if (objects[i].GetComponent<Renderer>() != null)
                {
            Debug.Log("8");
                    objects[i].GetComponent<Renderer>().material = newMaterial;
                }
            }
            Debug.Log("9");
            mediator.Notify(ReupEvent.objectMaterialChanged, message);
            //Dictionary<string, object> keyValuePairs = new Dictionary<string, object>
            //{
            //    { "material_urls", materialUrl },
            //    { "object_id", objectIds }
            //};
            //Debug.Log("notifiying");
            //mediator.Notify(ReupEvent.objectMaterialChanged, keyValuePairs);
        }
    }
}
