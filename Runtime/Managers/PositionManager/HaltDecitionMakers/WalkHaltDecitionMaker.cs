using UnityEngine;
using ReupVirtualTwin.managerInterfaces;

namespace ReupVirtualTwin.managers
{
    public class WalkHaltDecitionMaker : MovementHaltDecitionMaker<Vector3>
    {
        ICharacterPositionManager characterPositionManager;
        float stopMovementThreshold;

        public WalkHaltDecitionMaker(ICharacterPositionManager cpm, float stopThreshold)
        {
            characterPositionManager = cpm;
            stopMovementThreshold = stopThreshold;
        }

        public bool ShouldKeepMoving(Vector3 target)
        {
            var sameHeightTarget = new Vector3(target.x, characterPositionManager.characterPosition.y, target.z);
            bool thereIsDistanceToTarget = Vector3.Distance(sameHeightTarget, characterPositionManager.characterPosition) > stopMovementThreshold;
            return thereIsDistanceToTarget;

        }
    }
}
