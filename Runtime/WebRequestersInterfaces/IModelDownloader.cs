using System;

namespace ReupVirtualTwin.webRequestersInterfaces
{
    public interface IMeshDownloader
    {
        public void downloadMesh<T, E>(
            string meshUrl,
            Action<T, float> onProgress,
            Action<T> onLoad,
            Action<T> onMaterialsLoad,
            Action<E> onError = null
        );
    }
}
