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


        [SerializeField]
        private CharacterRotationManager _characterRotationManager;
        private IDragManager dragManager;
        private InputProvider inputProvider;

        [Inject]
        public void Init(InputProvider inputProvider, IDragManager dragManager)
        {
            this.inputProvider = inputProvider;
            this.dragManager = dragManager;
        }

        void Update()
        {
            if (dragManager.dragging)
            {
                Vector2 look = inputProvider.RotateViewInput();
                _characterRotationManager.horizontalRotation += (look.x * sensitivity);
                _characterRotationManager.verticalRotation += (look.y * sensitivity * -1f);
            }
        }

    }
}