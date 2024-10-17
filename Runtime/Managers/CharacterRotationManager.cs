using ReupVirtualTwin.managerInterfaces;
using UnityEngine;
namespace ReupVirtualTwin.managers
{
    public class CharacterRotationManager : MonoBehaviour, ICharacterRotationManager
    {
        // float ROTATION_SPEED = 10f;
        float ANGLE_THRESHOLD = 0.01f;
        float _verticalRotation = 0f;
        float _horizontalRotation = 0f;
        Quaternion _desiredInnerRotation;
        Quaternion _desiredHorizontalRotation;

        [SerializeField] Transform _innerCharacterTransform;

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
            Debug.Log("update character rotation");
            if (ShouldRotate())
            {
                Rotate();
            }
        }
        void SetDesiredHorizontalRotation()
        {
            // Debug.Log($"67: _horizontalRotation >>>\n{_horizontalRotation}");
            _desiredHorizontalRotation = Quaternion.Euler(0, _horizontalRotation, 0);
            // Debug.Log($"69: _desiredHorizontalRotation >>>\n{_desiredHorizontalRotation}");
        }
        void SetDesiredInnerRotation()
        {
            _desiredInnerRotation = Quaternion.Euler(_verticalRotation, transform.rotation.eulerAngles.y, 0);
        }

        bool ShouldRotate()
        {
            var shouldRotateVertically = Quaternion.Angle(_desiredInnerRotation, _innerCharacterTransform.rotation) > ANGLE_THRESHOLD;
            // Debug.Log($"79: shouldRotateVertically >>>\n{shouldRotateVertically}");
            // Debug.Log($"77: Quaternion.Angle(_desiredHorizontalRotation, transform.rotation) >>>\n{Quaternion.Angle(_desiredHorizontalRotation, transform.rotation)}");
            var shouldRotateHorizontally = Quaternion.Angle(_desiredHorizontalRotation, transform.rotation) > ANGLE_THRESHOLD;
            // Debug.Log($"79: shouldRotateHorizontally >>>\n{shouldRotateHorizontally}");
            return shouldRotateVertically || shouldRotateHorizontally;
        }

        void Rotate()
        {
            Debug.Log($"88: transform.forward >>>\n{transform.forward}");
            Debug.Log($"88: Camera.main.transform.forward >>>\n{Camera.main.transform.forward}");
            Debug.Log("rotating");
            // var rotationStep = ROTATION_SPEED * Time.deltaTime;
            SetDesiredInnerRotation();
            _innerCharacterTransform.rotation = _desiredInnerRotation;
            transform.rotation = _desiredHorizontalRotation;
            Debug.Log($"93: transform.forward >>>\n{transform.forward}");
            Debug.Log($"95: Camera.main.transform.forward >>>\n{Camera.main.transform.forward}");
        }
        // void RotateInnerCharacter(float rotationStep)
        // {
        //     // _innerCharacterTransform.rotation = Quaternion.Slerp(_innerCharacterTransform.rotation, _desiredInnerRotation, rotationStep);
        //     // _innerCharacterTransform.rotation = _desiredInnerRotation;
        // }
        // void RotateCharacter(float rotationStep)
        // {
        //     Debug.Log($"99: transform.rotation >>>\n{transform.rotation}");
        //     // transform.rotation = Quaternion.Slerp(transform.rotation, _desiredHorizontalRotation, rotationStep);
        //     transform.rotation = _desiredHorizontalRotation;
        //     Debug.Log($"102: transform.rotation >>>\n{transform.rotation}");
        // }
    }

}
