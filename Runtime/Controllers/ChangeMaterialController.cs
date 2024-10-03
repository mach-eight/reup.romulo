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
using Newtonsoft.Json.Schema;
using ReupVirtualTwin.managerInterfaces;

namespace ReupVirtualTwin.controllers
{
    public class ChangeMaterialController : IChangeMaterialController
    {
        public IMaterialScaler materialScaler = new MaterialScaler();
        public ITextureCompresser textureCompresser = new TextureCompresser();
        readonly ITextureDownloader textureDownloader;
        readonly IObjectRegistry objectRegistry;
        readonly ITexturesManager texturesManager;
        public ChangeMaterialController(ITextureDownloader textureDownloader, IObjectRegistry objectRegistry, ITexturesManager texturesManager)
        {
            this.textureDownloader = textureDownloader;
            this.objectRegistry = objectRegistry;
            this.texturesManager = texturesManager;
        }

        public async Task<TaskResult> ChangeObjectMaterial(JObject materialChangeInfo)
        {
            if (RomuloEnvironment.development)
            {
                if (!materialChangeInfo.IsValid(RomuloInternalSchema.materialChangeInfoSchema))
                {
                    return TaskResult.Failure("Error, the provided material change information does not conform to the expected schema");
                }
            }
            string materialUrl = materialChangeInfo["material"]["texture"].ToString();
            string[] objectIds = materialChangeInfo["objectIds"].ToObject<string[]>();
            float width = materialChangeInfo["material"]["widthMillimeters"].ToObject<float>();
            float height = materialChangeInfo["material"]["heightMillimeters"].ToObject<float>();
            Vector2 materialDimensionsInMillimeters = new Vector2(width, height);
            Texture2D texture = await textureDownloader.DownloadTextureFromUrl(materialUrl);
            if (!texture)
            {
                return TaskResult.Failure($"Error downloading image from {materialUrl}");
            }
            Texture2D compressedTexture = textureCompresser.GetASTC12x12CompressedTexture(texture);
            GameObject.Destroy(texture);
            List<GameObject> objects = objectRegistry.GetObjectsWithGuids(objectIds);

            Material newMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));

            newMaterial.SetTexture("_BaseMap", compressedTexture);

            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i].GetComponent<Renderer>() != null)
                {
                    texturesManager.ApplyMaterialToObject(objects[i], newMaterial);
                    objects[i].GetComponent<IObjectInfo>().materialWasChanged = true;
                    ObjectMetaDataUtils.AssignMaterialIdMetaDataToObject(objects[i], materialChangeInfo["material"]["id"].ToObject<int>());
                    materialScaler.AdjustUVScaleToDimensions(objects[i], materialDimensionsInMillimeters);
                }
            }
            return TaskResult.Success();
        }
    }
}
