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
using ReupVirtualTwin.helperInterfaces;

namespace ReupVirtualTwin.controllers
{
    public class ChangeMaterialController : IChangeMaterialController
    {
        public IMaterialScaler materialScaler = new MaterialScaler();
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
            string materialUrl = materialChangeInfo["material"]["texture"].ToString();
            string[] objectIds = materialChangeInfo["objectIds"].ToObject<string[]>();
            float width = materialChangeInfo["material"]["widthMilimeters"].ToObject<float>();
            float height = materialChangeInfo["material"]["heightMilimeters"].ToObject<float>();
            Vector2 materialDimensionsInMilimeters = new Vector2(width, height);
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
                    objects[i].GetComponent<IObjectInfo>().materialWasChanged = true;
                    ObjectMetaDataUtils.AssignMaterialIdMetaDataToObject(objects[i], materialChangeInfo["material"]["id"].ToObject<int>());
                    materialScaler.AdjustUVScaleToDimensions(objects[i], materialDimensionsInMilimeters);
                }
            }
            if (notify)
            {
                mediator.Notify(ReupEvent.objectMaterialChanged, materialChangeInfo);
            }
        }
    }
}
