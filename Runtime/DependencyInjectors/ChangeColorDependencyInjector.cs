using UnityEngine;

using RuntimeHandle;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.managers;
using ReupVirtualTwin.models;
using ReupVirtualTwin.helpers;

namespace ReupVirtualTwin.dependencyInjectors
{
    public class ChangeColorDependencyInjector : MonoBehaviour
    {
        [SerializeField]
        GameObject mediator;
        private void Awake()
        {
            ChangeColorManager changeColorManager = GetComponent<ChangeColorManager>();
            changeColorManager.mediator = mediator.GetComponent<IMediator>();
            changeColorManager.registry = ObjectFinder.FindObjectRegistry().GetComponent<IRegistry>();
        }
    }
}

