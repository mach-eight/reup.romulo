using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.webRequestersInterfaces;
using ReupVirtualTwin.dataModels;

namespace ReupVirtualTwin.controllers
{
    public class ChangeMaterialController : IChangeMaterialController
    {
        readonly ITextureDownloader textureDownloader;
        public ChangeMaterialController(ITextureDownloader textureDownloader)
        {
            this.textureDownloader = textureDownloader;
        }

        public void ChangeObjectMaterial(ChangeMaterialMessagePayload message)
        {
            this.textureDownloader.DownloadTextureFromUrl(message.material_url);
        }
    }
}
