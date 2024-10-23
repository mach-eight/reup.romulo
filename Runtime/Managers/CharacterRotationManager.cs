using ReupVirtualTwin.managerInterfaces;
using UnityEngine;
namespace ReupVirtualTwin.managers
{
    public class CharacterRotationManager : MonoBehaviour, ICharacterRotationManager
    {
        float ROTATION_SPEED = 10f;
        float ANGLE_THRESHOLD = 0.01f;
        float _verticalRotation = 0f;
        float _horizontalRotation = 0f;
        Quaternion _desiredInnerRotation;
        Quaternion _desiredHorizontalRotation;

        [SerializeField]
        Transform _innerCharacterTransform;

        bool _allowRotation = true;
        public bool allowRotation
        {
            set { _allowRotation = value; }
            get { return _allowRotation; }
        }

        public float verticalRotation
        {
            get
            {
                return _verticalRotation;
            }
            set
            {
                if (!_allowRotation) { return; }
                if (value > 180f) value -= 360f;
                _verticalRotation = Mathf.Clamp(value, -90f, 90f);
                SetDesiredInnerRotation();
            }
        }
        public float horizontalRotation
        {
            get
            {
                return _horizontalRotation;
            }
            set
            {
                if (!_allowRotation) { return; }
                _horizontalRotation = value;
                SetDesiredHorizontalRotation();
            }
        }

        private void Start()
        {
            verticalRotation = transform.rotation.eulerAngles.x;
            horizontalRotation = transform.rotation.eulerAngles.y;
        }

        void Update()
        {
            if (ShouldRotate())
            {
                Rotate();
            }
        }
        void SetDesiredHorizontalRotation()
        {
            _desiredHorizontalRotation = Quaternion.Euler(0, _horizontalRotation, 0);
        }
        void SetDesiredInnerRotation()
        {
            _desiredInnerRotation = Quaternion.Euler(_verticalRotation, transform.rotation.eulerAngles.y, 0);
        }

        bool ShouldRotate()
        {
            var shouldRotateVertically = Quaternion.Angle(_desiredInnerRotation, _innerCharacterTransform.rotation) > ANGLE_THRESHOLD;
            var shouldRotateHorizontally = Quaternion.Angle(_desiredHorizontalRotation, transform.rotation) > ANGLE_THRESHOLD;
            return shouldRotateVertically || shouldRotateHorizontally;
        }

        void Rotate()
        {
            var rotationStep = ROTATION_SPEED * Time.deltaTime;
            RotateCharacter(rotationStep);
            RotateInnerCharacter(rotationStep);
        }
        void RotateInnerCharacter(float rotationStep)
        {
            SetDesiredInnerRotation();
            _innerCharacterTransform.rotation = Quaternion.Slerp(_innerCharacterTransform.rotation, _desiredInnerRotation, rotationStep);
        }
        void RotateCharacter(float rotationStep)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _desiredHorizontalRotation, rotationStep);
        }
    }

}
