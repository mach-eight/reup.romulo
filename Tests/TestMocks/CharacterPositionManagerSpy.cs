using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ReupVirtualTwin.managerInterfaces;
using UnityEngine.Events;

namespace ReupVirtualTwinTests.mocks
{
    public class CharacterPositionManagerSpy : ICharacterPositionManager
    {
        public bool isKinematic = false;
        public int callsToMakeKinematic = 0;
        public bool allowWalking { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool allowSetHeight { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool allowMovingUp { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public float maxStepHeight { set => throw new System.NotImplementedException(); }
        public Vector3 characterPosition { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public void ApplyForceInDirection(Vector3 direction)
        {
            throw new System.NotImplementedException();
        }

        public void KeepHeight(float height)
        {
            throw new System.NotImplementedException();
        }

        public void MakeKinematic()
        {
            isKinematic = true;
            callsToMakeKinematic++;
        }

        public void MoveDistanceInDirection(float distance, Vector3 direction)
        {
            throw new System.NotImplementedException();
        }

        public void MoveInDirection(Vector3 direction, float speedInMetersPerSecond)
        {
            throw new System.NotImplementedException();
        }

        public void SlideToTarget(Vector3 target, UnityEvent endEvent)
        {
            throw new System.NotImplementedException();
        }

        public void SlideToTarget(Vector3 target)
        {
            throw new System.NotImplementedException();
        }

        public void StopRigidBody()
        {
            throw new System.NotImplementedException();
        }

        public void StopWalking()
        {
            throw new System.NotImplementedException();
        }

        public void UndoKinematic()
        {
            throw new System.NotImplementedException();
        }

        public void WalkToTarget(Vector3 target)
        {
            throw new System.NotImplementedException();
        }
    }
}
