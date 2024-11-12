using ReupVirtualTwin.helpers;
using ReupVirtualTwin.managerInterfaces;
using UnityEngine;
using Zenject;

namespace ReupVirtualTwin.managers
{
    public class CharacterRotationManager : ICharacterRotationManager, IInitializable, ILateTickable
    {
        public float ANGLE_THRESHOLD_TO_ROTATE { get; } = 0.01f;
        float _verticalRotation = 0f;
        float _horizontalRotation = 0f;
        Quaternion _desiredInnerRotation;
        Quaternion _desiredHorizontalRotation;
        Transform characterTransform;
        Transform innerCharacterTransform;
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

        public CharacterRotationManager(
            [Inject(Id = "character")] GameObject character,
            [Inject(Id = "innerCharacter")] GameObject innerCharacter)
        {
            characterTransform = character.transform;
            innerCharacterTransform = innerCharacter.transform;
        }

        public void Initialize()
        {
            verticalRotation = characterTransform.rotation.eulerAngles.x;
            horizontalRotation = characterTransform.rotation.eulerAngles.y;
        }

        public void LateTick()
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
            _desiredInnerRotation = Quaternion.Euler(_verticalRotation, characterTransform.rotation.eulerAngles.y, 0);
        }

        bool ShouldRotate()
        {
            var shouldRotateVertically = MathUtils.CalculateAngle(_desiredInnerRotation, innerCharacterTransform.rotation) > ANGLE_THRESHOLD_TO_ROTATE;
            var shouldRotateHorizontally = MathUtils.CalculateAngle(characterTransform.rotation, _desiredHorizontalRotation) > ANGLE_THRESHOLD_TO_ROTATE;
            return shouldRotateVertically || shouldRotateHorizontally;
        }

        void Rotate()
        {
            SetDesiredInnerRotation();
            innerCharacterTransform.rotation = _desiredInnerRotation;
            characterTransform.rotation = _desiredHorizontalRotation;
        }
    }
}
