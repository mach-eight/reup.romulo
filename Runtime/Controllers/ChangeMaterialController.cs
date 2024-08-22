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

        public async Task ChangeObjectMaterial(JObject materialChangeInfo, bool notify = true)
        {
            if (RomuloEnvironment.development)
            {
                if (!DataValidator.ValidateObjectToSchema(materialChangeInfo, RomuloInternalSchema.materialChangeInfo))
                {
                    return;
                }
            }
            string materialUrl = materialChangeInfo["material_url"].ToString();
            string[] objectIds = materialChangeInfo["object_ids"].ToObject<string[]>();
            float width_cm = materialChangeInfo["width_mm"].ToObject<long>() / 10;
            float height_cm = materialChangeInfo["height_mm"].ToObject<long>() / 10;
            Vector2 materialDimensionsInCm = new Vector2(width_cm, height_cm);
            Texture2D texture = await textureDownloader.DownloadTextureFromUrl(materialUrl);
            if (!texture)
            {
                mediator.Notify(ReupEvent.error, $"Error downloading image from {materialUrl}");
                return;
            }
            List<GameObject> objects = objectRegistry.GetObjectsWithGuids(objectIds);
            Material newMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            newMaterial.SetTexture("_BaseMap", texture);
            newMaterial.mainTextureScale = new Vector2(0.01f, 0.01f);
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i].GetComponent<Renderer>() != null)
                {
                    objects[i].GetComponent<Renderer>().material = newMaterial;
                    objects[i].GetComponent<IObjectInfo>().materialWasChanged = true;
                    ObjectMetaDataUtils.AssignMaterialIdMetaDataToObject(objects[i], materialChangeInfo["material_id"].ToObject<int>());
                    UvUtils.AdjustUVScaleToDimensions(objects[i], materialDimensionsInCm);
                }
            }
            if (notify)
            {
                mediator.Notify(ReupEvent.objectMaterialChanged, materialChangeInfo);
            }
        }
    }
}
