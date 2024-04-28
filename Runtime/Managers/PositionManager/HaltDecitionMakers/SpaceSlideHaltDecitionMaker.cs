using UnityEngine;
using ReupVirtualTwin.managerInterfaces;

namespace ReupVirtualTwin.managers
{
    public class SpaceSlideHaltDecitionMaker : MovementHaltDecitionMaker<Vector3>
    {
        ICharacterPositionManager characterPositionManager;
        float stopMovementThreshold;

        public SpaceSlideHaltDecitionMaker(ICharacterPositionManager cpm, float stopThreshold)
        {
            characterPositionManager = cpm;
            stopMovementThreshold = stopThreshold;
        }

        public bool ShouldKeepMoving(Vector3 target)
        {
            bool thereIsDistanceToTarget = Vector3.Distance(target, characterPositionManager.characterPosition) > stopMovementThreshold;
            return thereIsDistanceToTarget;

        }
    }
}
