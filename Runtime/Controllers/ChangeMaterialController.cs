using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.webRequestersInterfaces;
using ReupVirtualTwin.modelInterfaces;
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
        public ChangeMaterialController(ITextureDownloader textureDownloader, IObjectRegistry objectRegistry)
        {
            this.textureDownloader = textureDownloader;
            this.objectRegistry = objectRegistry;
        }

        public async Task<TaskResult> ChangeObjectMaterial(JObject materialChangeInfo)
        {
            if (RomuloEnvironment.development)
            {
                if (!DataValidator.ValidateObjectToSchema(materialChangeInfo, RomuloInternalSchema.materialChangeInfo))
                {
                    return TaskResult.Failure("Error, the provided material change information does not conform to the expected schema");
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
                return TaskResult.Failure($"Error downloading image from {materialUrl}");
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
            return TaskResult.Success();
        }
    }
}
