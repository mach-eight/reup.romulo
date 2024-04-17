using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.webRequestersInterfaces;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.modelInterfaces;

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

        public void ChangeObjectMaterial(ChangeMaterialMessagePayload message)
        {
            this.textureDownloader.DownloadTextureFromUrl(message.material_url);
            List<GameObject> objects = this.objectRegistry.GetObjectsWithGuids(message.object_ids);
            Material newMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
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
