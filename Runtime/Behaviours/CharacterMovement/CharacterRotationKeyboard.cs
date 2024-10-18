using UnityEngine;
using ReupVirtualTwin.inputs;
using ReupVirtualTwin.managers;
using Zenject;
using ReupVirtualTwin.managerInterfaces;

namespace ReupVirtualTwin.behaviours
{
    public class CharacterRotationKeyboard : MonoBehaviour
    {
        private ICharacterRotationManager characterRotationManager;

        private InputProvider inputProvider;

        private float ROTATION_SPEED_DEG_PER_SECOND = 180f;

        [Inject]
        public void Init(
            ICharacterRotationManager characterRotationManager,
            InputProvider inputProvider)
        {
            this.inputProvider = inputProvider;
            this.characterRotationManager = characterRotationManager;
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
            characterRotationManager.horizontalRotation += (look.x * deltaSpeed);
            characterRotationManager.verticalRotation += (look.y * deltaSpeed * -1);
        }
    }
}
