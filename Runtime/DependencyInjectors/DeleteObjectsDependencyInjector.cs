using UnityEngine;

using RuntimeHandle;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.managers;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.helpers;

namespace ReupVirtualTwin.dependencyInjectors
{
    public class DeleteObjectsDependencyInjector : MonoBehaviour
    {
        [SerializeField]
        GameObject mediator;
        private void Awake()
        {

            DeleteObjectsManager deleteObjectsManager = GetComponent<DeleteObjectsManager>();
            deleteObjectsManager.mediator = mediator.GetComponent<IMediator>();
            deleteObjectsManager.tagsController = new TagsController();
            deleteObjectsManager.registry = ObjectFinder.FindObjectRegistry().GetComponent<IObjectRegistry>();

        }
    }
}


