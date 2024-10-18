using ReupVirtualTwin.helpers;
using ReupVirtualTwin.inputs;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.managers;
using UnityEngine;
using Zenject;

namespace ReupVirtualTwin.behaviours
{
    public class PointerRotation : MonoBehaviour
    {
        public float sensitivity = 0.4f;


        private ICharacterRotationManager characterRotationManager;
        private IDragManager dragManager;
        private InputProvider inputProvider;

        [Inject]
        public void Init(InputProvider inputProvider, IDragManager dragManager, ICharacterRotationManager characterRotationManager)
        {
            this.inputProvider = inputProvider;
            this.dragManager = dragManager;
            this.characterRotationManager = characterRotationManager;
        }

        void Update()
        {
            if (dragManager.dragging)
            {
                Vector2 look = inputProvider.RotateViewInput();
                characterRotationManager.horizontalRotation += (look.x * sensitivity);
                characterRotationManager.verticalRotation += (look.y * sensitivity * -1f);
            }
        }

    }
}