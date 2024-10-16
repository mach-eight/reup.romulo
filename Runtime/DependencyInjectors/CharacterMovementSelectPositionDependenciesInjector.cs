using UnityEngine;

using ReupVirtualTwin.helpers;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.behaviours;

namespace ReupVirtualTwin.dependencyInjectors
{
    [RequireComponent(typeof(CharacterMovementSelectPosition))]
    public class CharacterMovementSelectPositionDependenciesInjector : MonoBehaviour
    {
        void Start()
        {
            CharacterMovementSelectPosition characterMovementSelectPosition = GetComponent<CharacterMovementSelectPosition>();
            IEditModeManager editModeManager = ObjectFinder.FindEditModeManager().GetComponent<IEditModeManager>();
            characterMovementSelectPosition.editModeManager = editModeManager;
        }
    }
}
