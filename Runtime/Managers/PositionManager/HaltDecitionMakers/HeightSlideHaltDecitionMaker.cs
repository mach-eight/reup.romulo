using UnityEngine;
using ReupVirtualTwin.managerInterfaces;

namespace ReupVirtualTwin.managers
{
    public class HeightSlideHaltDecitionMaker : MovementHaltDecitionMaker<float>
    {
        ICharacterPositionManager characterPositionManager;
        float stopMovementThreshold;

        public HeightSlideHaltDecitionMaker(ICharacterPositionManager cpm, float stopThreshold)
        {
            characterPositionManager = cpm;
            stopMovementThreshold = stopThreshold;
        }
        public bool ShouldKeepMoving(float target)
        {
            float distanceToHeight = Mathf.Abs(target - characterPositionManager.characterPosition.y);
            bool thereIsDistanceToTarget = distanceToHeight > stopMovementThreshold;
            return thereIsDistanceToTarget;
        }

    }
}
