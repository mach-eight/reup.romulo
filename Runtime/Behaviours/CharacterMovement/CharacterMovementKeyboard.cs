using UnityEngine;
using ReupVirtualTwin.inputs;
using ReupVirtualTwin.managerInterfaces;
using Zenject;

namespace ReupVirtualTwin.behaviours
{
    public class CharacterMovementKeyboard : MonoBehaviour
    {
        private Transform _innerCharacterTransform;
        public Transform innerCharacterTransform { set => _innerCharacterTransform = value; }

        private InputProvider _inputProvider;
        private ICharacterPositionManager characterPositionManager;

        [Inject]
        public void Init(ICharacterPositionManager characterPositionManager)
        {
            this.characterPositionManager = characterPositionManager;
        }

        static public float WALK_SPEED_M_PER_SECOND = 3.5f;


        private void Awake()
        {
            _inputProvider = new InputProvider();
        }

        private void Update()
        {
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            Vector2 inputValue = _inputProvider.MovementInput().normalized;
            PerformMovement(inputValue);
        }

        private void PerformMovement(Vector2 direction)
        {
            Vector3 movementDirection = direction.y * GetCharacterForward() + direction.x * GetCharacterRight();
            if (movementDirection != Vector3.zero && characterPositionManager.allowWalking)
            {
                characterPositionManager.StopWalking();
                characterPositionManager.MoveInDirection(movementDirection, WALK_SPEED_M_PER_SECOND);
            }
        }

        private Vector3 GetCharacterForward()
        {
            Vector3 forward = _innerCharacterTransform.forward;
            forward.y = 0;
            return forward;
        }
        private Vector3 GetCharacterRight()
        {
            Vector3 right = _innerCharacterTransform.right;
            right.y = 0;
            return right;
        }
    }
}
