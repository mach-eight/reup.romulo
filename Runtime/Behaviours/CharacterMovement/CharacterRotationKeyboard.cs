using UnityEngine;
using ReupVirtualTwin.inputs;
using ReupVirtualTwin.managers;

namespace ReupVirtualTwin.behaviours
{
    public class CharacterRotationKeyboard : MonoBehaviour
    {
        [SerializeField]
        private Transform _innerCharacterTransform;
        [SerializeField]
        private CharacterRotationManager _characterRotationManager;

        private InputProvider _inputProvider;

        private float ROTATION_SPEED_DEG_PER_SECOND = 180f;

        private void Awake()
        {
            _inputProvider = new InputProvider();
        }

        private void Update()
        {
            UpdateRotation();
        }
        private void UpdateRotation()
        {
            Vector2 look = _inputProvider.RotateViewKeyboardInput();
            float deltaSpeed = ROTATION_SPEED_DEG_PER_SECOND * Time.deltaTime;
            _characterRotationManager.horizontalRotation += (look.x * deltaSpeed);
            _characterRotationManager.verticalRotation += (look.y * deltaSpeed * -1);
        }
    }
}
