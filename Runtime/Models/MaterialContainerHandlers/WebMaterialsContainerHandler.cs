using UnityEngine;
using ReupVirtualTwin.helpers;

namespace ReupVirtualTwin.models
{
    public class WebMaterialsContainerHandler : MonoBehaviour, IWebMaterialContainerHandler
    {
        [SerializeField]
        GameObject webMaterialHandlerPrefab;
        IObjectPool _objectPool;
        GameObject webMaterialsHandler;

        private void Start()
        {
            _objectPool = ObjectFinder.FindObjectPool();
        }
        public GameObject CreateContainer()
        {
            if (webMaterialsHandler != null)
            {
                return null;
            }
            webMaterialsHandler = _objectPool.GetObjectFromPool(webMaterialHandlerPrefab.name, transform);
            webMaterialsHandler.transform.position = Vector3.zero;
            return webMaterialsHandler;
        }

        public void HideContainer()
        {
            _objectPool.PoolObject(webMaterialsHandler);
            webMaterialsHandler = null;
        }
    }
}
