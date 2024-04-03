using System;
using ReupVirtualTwin.dataModels;

namespace ReupVirtualTwin.webRequestersInterfaces
{
    public interface IMeshDownloader
    {
        //public void downloadMesh<T, E>(
        //    string meshUrl,
        //    Action<T, float> onProgress,
        //    Action<T> onLoad,
        //    Action<T> onMaterialsLoad,
        //    Action<E> onError = null
        //);

        public void downloadMesh(
            string meshUrl,
            Action<ModelLoaderContext, float> onProgress,
            Action<ModelLoaderContext> onLoad,
            Action<ModelLoaderContext> onMaterialsLoad
            //Action onError = null
        );
    }
}
