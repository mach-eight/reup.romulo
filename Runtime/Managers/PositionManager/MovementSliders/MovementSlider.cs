using System.Collections;
using UnityEngine;
using System;
using UnityEngine.Events;

using ReupVirtualTwin.helpers;
using ReupVirtualTwin.managerInterfaces;

namespace ReupVirtualTwin.managers
{
    public class MovementSlider<T> : MonoBehaviour where T : IEquatable<T>
    {
        public bool sliding = false;
        public MovementHaltDecitionMaker<T> movementDecitionMaker;
        public Interpolator<T> interpolator;

        UnityEvent endMovementEvent; 
        T currentTarget;

        ICharacterPositionManager _positionManager;

        void Awake()
        {
            _positionManager = GetComponent<ICharacterPositionManager>();
        }
        public MovementSlider<T> SetHaltDecitionMaker(MovementHaltDecitionMaker<T> mhdm)
        {
            movementDecitionMaker = mhdm;
            return this;
        }
        public MovementSlider<T> SetInterpolator(Interpolator<T> interp)
        {
            interpolator = interp;
            return this;
        }
        public void SlideToTarget(T target, UnityEvent endMovementEvent)
        {
            this.endMovementEvent = endMovementEvent;
            SlideToTarget(target);
        }
        public void SlideToTarget(T target)
        {
            if (sliding)
            {
                if(currentTarget.Equals(target))
                {
                    return;
                }
                else
                {
                    StopCoroutine("SlideToTargetCoroutine");
                }
            }
            currentTarget = target;
            StartCoroutine("SlideToTargetCoroutine", target);
        }
        private IEnumerator SlideToTargetCoroutine(T target)
        {
            sliding = true;
            interpolator.DefineOriginAndTarget(_positionManager.characterPosition, target);
            while (movementDecitionMaker.ShouldKeepMoving(target))
            {
                var nextPos = interpolator.Interpolate(_positionManager.characterPosition);
                _positionManager.characterPosition = nextPos;
                yield return null;
            }
            StopMovement();
            if (endMovementEvent != null)
            {
                endMovementEvent.Invoke();
            }
        }

        public void StopMovement()
        {
            StopCoroutine("SlideToTargetCoroutine");
            sliding = false;
        }
        public bool ShouldKeepMoving(T target)
        {
            return movementDecitionMaker.ShouldKeepMoving(target);
        }
    }
}
