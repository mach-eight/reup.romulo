using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.webRequestersInterfaces;
using System;
using TriLibCore;
using UnityEngine;

namespace ReupVirtualTwin.webRequesters
{
    public class MeshDownloader : IMeshDownloader
    {
        public void downloadMesh(string meshUrl, Action<ModelLoaderContext, float> onProgress, Action<ModelLoaderContext> onLoad, Action<ModelLoaderContext> onMaterialsLoad)
        {
            var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
            var webRequest = AssetDownloader.CreateWebRequest(meshUrl);
            AssetDownloader.LoadModelFromUri(
                webRequest,
                (AssetLoaderContext context) => ExecuteOnLoad(onLoad, context),
                (AssetLoaderContext context) => ExecuteOnMaterialsLoad(onMaterialsLoad, context),
                (AssetLoaderContext context, float progress) => ExecuteOnProgress(onProgress, context, progress),
                (IContextualizedError error) => { Debug.LogError(error); },
                null,
                assetLoaderOptions,
                null,
                "fbx"
            );
        }
        private void ExecuteOnProgress(Action<ModelLoaderContext, float> onProgress, AssetLoaderContext assetLoaderContext, float progress)
        {
            onProgress(new ModelLoaderContext() { loadedObject = assetLoaderContext?.RootGameObject }, progress);
        }
        private void ExecuteOnLoad(Action<ModelLoaderContext> onLoad, AssetLoaderContext assetLoaderContext)
        {
            onLoad(new ModelLoaderContext() { loadedObject = assetLoaderContext?.RootGameObject });
        }
        private void ExecuteOnMaterialsLoad(Action<ModelLoaderContext> onMaterialsLoad, AssetLoaderContext assetLoaderContext)
        {
            onMaterialsLoad(new ModelLoaderContext() { loadedObject = assetLoaderContext?.RootGameObject });
        }
    }
}
