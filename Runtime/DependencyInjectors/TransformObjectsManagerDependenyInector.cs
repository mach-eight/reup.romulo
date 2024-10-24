using UnityEngine;

using RuntimeHandle;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.managers;

namespace ReupVirtualTwin.dependencyInjectors
{
    public class TransformObjectsManagerDependenyInector : MonoBehaviour
    {
        [SerializeField]
        GameObject mediator;
        GameObject transformHandleObj;
        private void Awake()
        {
            TransformObjectsManager transformObjectsManager = GetComponent<TransformObjectsManager>();
            transformObjectsManager.mediator = mediator.GetComponent<IMediator>();
            transformHandleObj = new GameObject("TransformHandle");
            transformHandleObj.AddComponent<RuntimeTransformHandle>();
            transformObjectsManager.runtimeTransformObj = transformHandleObj;
            transformObjectsManager.tagsController = new TagsController();
        }
        private void OnDestroy() 
        {
            Destroy(transformHandleObj);
        }
    }
}
