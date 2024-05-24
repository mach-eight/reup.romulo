using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.webRequestersInterfaces;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.helpers;
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
            if (RomuloEnvironment.development)
            {
                if (!DataValidator.ValidateObjectToSchema(message, RomuloInternalSchema.materialChangeInfo))
                {
                    return;
                }
            }
            string materialUrl = message["material_url"].ToString();
            string[] objectIds = message["object_ids"].ToObject<string[]>();
            Texture2D texture = await textureDownloader.DownloadTextureFromUrl(materialUrl);
            if (!texture)
            {
                mediator.Notify(ReupEvent.error, $"Error downloading image from {materialUrl}");
                return;
            }
            List<GameObject> objects = objectRegistry.GetObjectsWithGuids(objectIds);
            Material newMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            newMaterial.SetTexture("_BaseMap", texture);
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i].GetComponent<Renderer>() != null)
                {
                    objects[i].GetComponent<Renderer>().material = newMaterial;
                }
            }
            mediator.Notify(ReupEvent.objectMaterialChanged, message);
        }
    }
}
