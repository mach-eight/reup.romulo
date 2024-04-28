using UnityEngine;
using ReupVirtualTwin.helpers;
using UnityEngine.Events;
using ReupVirtualTwin.managerInterfaces;

namespace ReupVirtualTwin.managers
{
    public class CharacterPositionManager : MonoBehaviour, ICharacterPositionManager
    {
        bool _allowSetHeight = true;
        bool _allowWalking = true;
        bool _allowMovingUp = true;
        private float _maxStepHeight;
        private float movementForce = 20f;
        private Rigidbody rb;
        private float bodyDrag = 5f;
        private float bodyMass = 1f;

        float STOP_WALK_THRESHOLD = 0.5f;
        float STOP_MOVEMENT_THRESHOLD = 0.02f;

        SpaceSlider walkSlider;
        SpaceSlider spaceSlider;
        LinearSlider heightSlider;

        public float maxStepHeight { set => _maxStepHeight = value; }
        public bool allowMovingUp
        {
            get { return _allowMovingUp; }
            set
            {
                _allowMovingUp = value;
                if (_allowMovingUp == false)
                {
                    heightSlider.StopMovement();
                    walkSlider.StopMovement();
                }
            }
        }
        public bool allowWalking
        {
            get { return _allowWalking; }
            set
            {
                if (value == false)
                {
                    walkSlider.StopMovement();
                }
                _allowWalking=value;
            }
        }
        public bool allowSetHeight
        {
            get { return _allowSetHeight; }
            set
            {
                if (value == false)
                {
                    heightSlider.StopMovement();
                }
                _allowSetHeight=value;
            }
        }

        public Vector3 characterPosition
        {
            get
            {
                return transform.position;
            }
            set
            {
                transform.position = value;
            }
        }

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
            rb.drag = bodyDrag;
            rb.mass = bodyMass;
            DefineWalkSlider();
            DefineSpaceSlider();
            DefineHeightSlider();
        }


        void DefineWalkSlider()
        {
            walkSlider = (SpaceSlider)transform.gameObject.AddComponent<SpaceSlider>()
                .SetHaltDecitionMaker(new WalkHaltDecitionMaker(this, STOP_WALK_THRESHOLD))
                .SetInterpolator(new WalkInterpolator());
        }
        void DefineSpaceSlider()
        {
            spaceSlider = (SpaceSlider)transform.gameObject.AddComponent<SpaceSlider>()
                .SetHaltDecitionMaker(new SpaceSlideHaltDecitionMaker(this, STOP_MOVEMENT_THRESHOLD))
                .SetInterpolator(new SpacesInterpolator());
        }
        void DefineHeightSlider()
        {
            heightSlider = (LinearSlider)transform.gameObject.AddComponent<LinearSlider>()
                .SetHaltDecitionMaker(new HeightSlideHaltDecitionMaker(this, STOP_MOVEMENT_THRESHOLD))
                .SetInterpolator(new HeightInterpolator());
        }

        public void MoveDistanceInDirection(float distance, Vector3 direction)
        {
            var normalizedDirection = Vector3.Normalize(direction);
            characterPosition = characterPosition + normalizedDirection * distance;
        }

        public void MoveInDirection(Vector3 direction, float speedInMetersPerSecond = 1f)
        {
            var normalizedDirection = Vector3.Normalize(direction);
            characterPosition = characterPosition + normalizedDirection * speedInMetersPerSecond * Time.deltaTime;
        }
        public void ApplyForceInDirection(Vector3 direction)
        {
            var force = Vector3.Normalize(direction) * movementForce;
            rb.AddForce(force, ForceMode.Force);
        }
        public void WalkToTarget(Vector3 target)
        {
            walkSlider.SlideToTarget(target);
        }

        public void SlideToTarget(Vector3 target, UnityEvent endEvent)
        {
            spaceSlider.SlideToTarget(target, endEvent);
        }
        public void SlideToTarget(Vector3 target)
        {
            walkSlider.StopMovement();
            heightSlider.StopMovement();
            spaceSlider.SlideToTarget(target);
        }

        public void KeepHeight(float height)
        {
            if (ShouldSetHeight(height))
            {
                MoveToHeight(height);
            }
        }

        private void MoveToHeight(float height)
        {
            heightSlider.SlideToTarget(height);
        }

        bool ShouldSetHeight(float target)
        {
            if (false
                || !allowSetHeight
                || spaceSlider.sliding
                || IsStronglyGoingUp(target)
                || (!allowMovingUp && target > characterPosition.y)
            )
            {
                return false;
            }
            return heightSlider.ShouldKeepMoving(target);
        }
        private bool IsStronglyGoingUp(float target)
        {
            if (target - characterPosition.y > _maxStepHeight)
            {
                Debug.LogWarning($"Character is trying to go up too much {target - characterPosition.y} m. Max step allows is {_maxStepHeight} m.");
                return true;
            }
            return false;
        }

        public void StopRigidBody()
        {
            rb.velocity = Vector3.zero;
        }

        public void StopWalking()
        {
            walkSlider.StopMovement();
        }

        public void MakeKinematic()
        {
            rb.isKinematic = true;
        }
        public void UndoKinematic()
        {
            rb.isKinematic = false;
        }


    }
}
