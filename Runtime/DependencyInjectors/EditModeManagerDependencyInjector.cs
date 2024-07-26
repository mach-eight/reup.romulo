using UnityEngine;

using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.managers;

namespace ReupVirtualTwin.dependencyInjectors
{
    public class EditModeManagerDependencyInjector : MonoBehaviour
    {
        EditModeManager _editModeManager;
        [SerializeField]
        GameObject editMediator;
        private void Awake()
        {
            _editModeManager = GetComponent<EditModeManager>();
            _editModeManager.mediator = editMediator.GetComponent<IMediator>();
        }
    }
}
