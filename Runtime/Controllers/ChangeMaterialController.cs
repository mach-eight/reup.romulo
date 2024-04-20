using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.webRequestersInterfaces;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.modelInterfaces;
using System.Threading.Tasks;

namespace ReupVirtualTwin.controllers
{
    public class ChangeMaterialController : IChangeMaterialController
    {
        readonly ITextureDownloader textureDownloader;
        readonly IObjectRegistry objectRegistry;
        public ChangeMaterialController(ITextureDownloader textureDownloader, IObjectRegistry objectRegistry)
        {
            this.textureDownloader = textureDownloader;
            this.objectRegistry = objectRegistry;
        }

        public async Task ChangeObjectMaterial(ChangeMaterialMessagePayload message)
        {
            Texture2D texture = await this.textureDownloader.DownloadTextureFromUrl(message.material_url);
            List<GameObject> objects = this.objectRegistry.GetObjectsWithGuids(message.object_ids);
            Material newMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            newMaterial.SetTexture("_BaseMap", texture);
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i].GetComponent<Renderer>() != null)
                {
                    objects[i].GetComponent<Renderer>().material = newMaterial;
                }
            }
        }
    }
}
