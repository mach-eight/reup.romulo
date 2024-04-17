using System;
using ReupVirtualTwin.dataModels;

namespace ReupVirtualTwin.webRequestersInterfaces
{
    public interface IMeshDownloader
    {
        public void downloadMesh(
            string meshUrl,
            Action<ModelLoaderContext, float> onProgress,
            Action<ModelLoaderContext> onLoad,
            Action<ModelLoaderContext> onMaterialsLoad
        );
    }
}
