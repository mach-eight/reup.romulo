using UnityEngine;

using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.managers;

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
        }
    }
}

