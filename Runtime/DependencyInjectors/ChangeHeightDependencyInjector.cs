using UnityEngine;

using ReupVirtualTwin.behaviours;
using ReupVirtualTwin.managerInterfaces;

namespace ReupVirtualTwin.dependencyInjectors
{
    public class ChangeHeightDependencyInjector : MonoBehaviour
    {
        [SerializeField]
        GameObject heightMediator;

        private void Awake()
        {
            ChangeHeight changeHeight = GetComponent<ChangeHeight>();
            changeHeight.mediator = heightMediator.GetComponent<IMediator>();
        }
    }
}
