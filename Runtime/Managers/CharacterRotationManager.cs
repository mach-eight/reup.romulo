using ReupVirtualTwin.helpers;
using ReupVirtualTwin.managerInterfaces;
using UnityEngine;
namespace ReupVirtualTwin.managers
{
    public class CharacterRotationManager : MonoBehaviour, ICharacterRotationManager
    {
        float ANGLE_THRESHOLD = 0.01f;
        float _verticalRotation = 0f;
        float _horizontalRotation = 0f;
        Quaternion _desiredInnerRotation;
        Quaternion _desiredHorizontalRotation;
        [SerializeField] Transform _innerCharacterTransform;
        public bool allowRotation { get; set; } = true;

        public float verticalRotation
        {
            get
            {
                return _verticalRotation;
            }
            set
            {
                if (!allowRotation) { return; }
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
                if (!allowRotation) { return; }
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
            var shouldRotateVertically = MathUtils.CalculateAngle(_desiredInnerRotation, _innerCharacterTransform.rotation) > ANGLE_THRESHOLD;
            var shouldRotateHorizontally = MathUtils.CalculateAngle(transform.rotation, _desiredHorizontalRotation) > ANGLE_THRESHOLD;
            return shouldRotateVertically || shouldRotateHorizontally;
        }

        void Rotate()
        {
            SetDesiredInnerRotation();
            _innerCharacterTransform.rotation = _desiredInnerRotation;
            transform.rotation = _desiredHorizontalRotation;
        }
    }
}
