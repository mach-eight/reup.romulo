using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.webRequestersInterfaces;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.enums;

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

        public async Task ChangeObjectMaterial(ChangeMaterialMessagePayload message)
        {
            Texture2D texture = await textureDownloader.DownloadTextureFromUrl(message.material_url);
            if (!texture)
            {
                mediator.Notify(ReupEvent.error, $"Error downloading image from {message.material_url}");
                return;
            }
            List<GameObject> objects = objectRegistry.GetObjectsWithGuids(message.object_ids);
            Material newMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            newMaterial.SetTexture("_BaseMap", texture);
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i].GetComponent<Renderer>() != null)
                {
                    objects[i].GetComponent<Renderer>().material = newMaterial;
                }
            }
            ChangeMaterialMessagePayload materialChangeInfo = new()
            {
                object_ids = message.object_ids,
                material_url = message.material_url,
            };
            mediator.Notify(ReupEvent.objectMaterialChanged, materialChangeInfo);
        }
    }
}