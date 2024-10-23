using ReupVirtualTwin.helpers;
using ReupVirtualTwin.inputs;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.managers;
using UnityEngine;

namespace ReupVirtualTwin.behaviours
{
    public class PointerRotation : MonoBehaviour
    {
        public float sensitivity = 0.4f;


        [SerializeField]
        private CharacterRotationManager _characterRotationManager;
        private IDragManager _dragManager;
        private InputProvider _inputProvider;

        private void Awake()
        {
            _inputProvider = new InputProvider();
            _dragManager = ObjectFinder.FindDragManager().GetComponent<IDragManager>();
        }


        void Update()
        {
            if (_dragManager.dragging)
            {
                Vector2 look = _inputProvider.RotateViewInput();
                _characterRotationManager.horizontalRotation += (look.x * sensitivity);
                _characterRotationManager.verticalRotation += (look.y * sensitivity * -1f);
            }
        }

    }
}