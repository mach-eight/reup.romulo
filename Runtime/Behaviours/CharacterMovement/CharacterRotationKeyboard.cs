using UnityEngine;
using ReupVirtualTwin.inputs;
using ReupVirtualTwin.managers;
using Zenject;

namespace ReupVirtualTwin.behaviours
{
    public class CharacterRotationKeyboard : MonoBehaviour
    {
        [SerializeField]
        private Transform _innerCharacterTransform;
        [SerializeField]
        private CharacterRotationManager _characterRotationManager;

        private InputProvider inputProvider;

        private float ROTATION_SPEED_DEG_PER_SECOND = 180f;

        [Inject]
        public void Init(InputProvider inputProvider)
        {
            this.inputProvider = inputProvider;
        }

        private void Update()
        {
            UpdateRotation();
        }
        private void UpdateRotation()
        {
            Vector2 look = inputProvider.RotateViewKeyboardInput();
            if (look == Vector2.zero)
            {
                return;
            }
            float deltaSpeed = ROTATION_SPEED_DEG_PER_SECOND * Time.deltaTime;
            _characterRotationManager.horizontalRotation += (look.x * deltaSpeed);
            _characterRotationManager.verticalRotation += (look.y * deltaSpeed * -1);
        }
    }
}
