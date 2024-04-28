using UnityEngine;
using ReupVirtualTwin.managerInterfaces;

namespace ReupVirtualTwin.behaviours
{
    public class CharacterMovementSelectPosition : SelectPoint
    {
        private IEditModeManager _editModeManager;
        private ICharacterPositionManager _characterPositionManager;

        public override void HandleHit(RaycastHit hit)
        {
            if (_characterPositionManager.allowWalking && _editModeManager.editMode == false)
            {
                _characterPositionManager.WalkToTarget(hit.point);
            }
        }
        public IEditModeManager editModeManager
        {
            set { _editModeManager = value; }
        }
        public ICharacterPositionManager characterPositionManager
        {
            set { _characterPositionManager = value; }
        }
    }
}
