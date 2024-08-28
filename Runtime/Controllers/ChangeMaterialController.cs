using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.webRequestersInterfaces;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.dataModels;
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
        public ChangeMaterialController(ITextureDownloader textureDownloader, IObjectRegistry objectRegistry, IMediator mediator)
        {
            this.textureDownloader = textureDownloader;
            this.objectRegistry = objectRegistry;
        }

        public async Task<Result> ChangeObjectMaterial(JObject materialChangeInfo)
        {
            if (RomuloEnvironment.development)
            {
                if (!DataValidator.ValidateObjectToSchema(materialChangeInfo, RomuloInternalSchema.materialChangeInfo))
                {
                    return Result.Failure("Error, the provided material change information does not conform to the expected schema");
                }
            }
            string materialUrl = materialChangeInfo["material_url"].ToString();
            string[] objectIds = materialChangeInfo["object_ids"].ToObject<string[]>();
            float width = materialChangeInfo["width_mm"].ToObject<float>();
            float height = materialChangeInfo["height_mm"].ToObject<float>();
            Vector2 materialDimensionsInMilimeters = new Vector2(width, height);
            Texture2D texture = await textureDownloader.DownloadTextureFromUrl(materialUrl);
            if (!texture)
            {
                return Result.Failure($"Error downloading image from {materialUrl}");
            }
            List<GameObject> objects = objectRegistry.GetObjectsWithGuids(objectIds);
            if (objects.Count == 0)
            {
                return Result.Failure("No objects found matching the given ID");
            }
            Material newMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            newMaterial.SetTexture("_BaseMap", texture);

            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i].GetComponent<Renderer>() != null)
                {
                    objects[i].GetComponent<Renderer>().material = newMaterial;
                    objects[i].GetComponent<IObjectInfo>().materialWasChanged = true;
                    ObjectMetaDataUtils.AssignMaterialIdMetaDataToObject(objects[i], materialChangeInfo["material_id"].ToObject<int>());
                    materialScaler.AdjustUVScaleToDimensions(objects[i], materialDimensionsInMilimeters);
                }
            }
            return Result.Success();
        }
    }
}
