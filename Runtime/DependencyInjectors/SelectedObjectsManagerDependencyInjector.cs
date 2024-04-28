using UnityEngine;

using ReupVirtualTwin.helpers;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.managers;
using ReupVirtualTwin.controllers;

namespace ReupVirtualTwin.dependencyInjectors
{
    public class SelectedObjectsManagerDependencyInjector : MonoBehaviour
    {
        [SerializeField]
        GameObject mediator;
        private void Start()
        {
            SelectedObjectsManager selectedObjectsManager = GetComponent<SelectedObjectsManager>();
            selectedObjectsManager.mediator = mediator.GetComponent<IMediator>();
            selectedObjectsManager.objectWrapper = new ObjectWrapper();
            Outliner outliner = gameObject.AddComponent<Outliner>();
            selectedObjectsManager.highlighter = outliner;

            SelectableObjectSelector selector = GetComponent<SelectableObjectSelector>();
            selector.tagsController = new TagsController();
        }
    }
}
