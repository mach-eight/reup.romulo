using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.webRequestersInterfaces;
using System;
using TriLibCore;
using UnityEngine;

namespace ReupVirtualTwin.webRequesters
{
    public class MeshDownloader : IMeshDownloader
    {
        //public void downloadMesh<T, E>(string meshUrl, Action<T, float> onProgress, Action<T> onLoad, Action<T> onMaterialsLoad, Action<E> onError = null)
        //{
        //    var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
        //    var webRequest = AssetDownloader.CreateWebRequest(meshUrl);
        //    AssetDownloader.LoadModelFromUri(
        //        webRequest,
        //        (Action<AssetLoaderContext>)(object)onLoad,
        //        (Action<AssetLoaderContext>)(object)onMaterialsLoad,
        //        (Action<AssetLoaderContext, float>)(object)onProgress,
        //        (Action<IContextualizedError>)(object)onError,
        //        null,
        //        assetLoaderOptions,
        //        null,
        //        "fbx"
        //    );
        //}
        public void downloadMesh(string meshUrl, Action<ModelLoaderContext, float> onProgress, Action<ModelLoaderContext> onLoad, Action<ModelLoaderContext> onMaterialsLoad)
        {
            var assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions();
            var webRequest = AssetDownloader.CreateWebRequest(meshUrl);
            AssetDownloader.LoadModelFromUri(
                webRequest,
                (AssetLoaderContext context) => onLoad(new ModelLoaderContext() { loadedObject = context.RootGameObject }),
                (AssetLoaderContext context) => onMaterialsLoad(new ModelLoaderContext() { loadedObject = context.RootGameObject }),
                (AssetLoaderContext context, float progress) => onProgress(new ModelLoaderContext() { loadedObject = context.RootGameObject }, progress),
                (IContextualizedError error) => { Debug.Log(error); },
                null,
                assetLoaderOptions,
                null,
                "fbx"
            );
        }
    }
}
