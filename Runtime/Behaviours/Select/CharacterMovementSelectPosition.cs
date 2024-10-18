using UnityEngine;
using ReupVirtualTwin.managerInterfaces;
using Zenject;

namespace ReupVirtualTwin.behaviours
{
    public class CharacterMovementSelectPosition : SelectPoint
    {
        private IEditModeManager _editModeManager;
        private ICharacterPositionManager characterPositionManager;

        [Inject]
        public void Init(ICharacterPositionManager characterPositionManager)
        {
            this.characterPositionManager = characterPositionManager;
        }

        public override void HandleHit(RaycastHit hit)
        {
            if (characterPositionManager.allowWalking && _editModeManager.editMode == false)
            {
                characterPositionManager.WalkToTarget(hit.point);
            }
        }
        public IEditModeManager editModeManager
        {
            set { _editModeManager = value; }
        }
    }
}
