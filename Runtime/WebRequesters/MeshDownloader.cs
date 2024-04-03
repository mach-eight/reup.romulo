using ReupVirtualTwin.webRequestersInterfaces;
using System;
using TriLibCore;

namespace ReupVirtualTwin.webRequesters
{
    public class MeshDownloader : IMeshDownloader
    {
        public void downloadMesh<T, E>(string meshUrl, Action<T, float> onProgress, Action<T> onLoad, Action<T> onMaterialsLoad, Action<E> onError = null)
        {
            var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
            var webRequest = AssetDownloader.CreateWebRequest(meshUrl);
            AssetDownloader.LoadModelFromUri(
                webRequest,
                (Action<AssetLoaderContext>)(object)onLoad,
                (Action<AssetLoaderContext>)(object)onMaterialsLoad,
                (Action<AssetLoaderContext, float>)(object)onProgress,
                (Action<IContextualizedError>)(object)onError,
                null,
                assetLoaderOptions,
                null,
                "fbx"
            );
        }
    }
}
