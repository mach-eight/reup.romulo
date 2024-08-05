using UnityEngine;

using ReupVirtualTwin.helpers;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.managers;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.romuloEnvironment;
using ReupVirtualTwin.modelInterfaces;

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
            Outliner outliner = new Outliner(RomuloEnvironment.reupBlueColor, 5.0f);
            selectedObjectsManager.highlighter = outliner;

            SelectableObjectSelector selector = GetComponent<SelectableObjectSelector>();
            selector.tagsController = new TagsController();

            selectedObjectsManager.highlightAnimator = new HighlightAnimator();
            IObjectRegistry registry = ObjectFinder.FindObjectRegistry().GetComponent<IObjectRegistry>();
            selectedObjectsManager.objectRegistry = registry;
            selectedObjectsManager.tagsController = new TagsController();
        }
    }
}
