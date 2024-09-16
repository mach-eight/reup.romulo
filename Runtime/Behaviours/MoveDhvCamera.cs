using ReupVirtualTwin.inputs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.behaviours
{
    public class MoveDhvCamera : MonoBehaviour
    {
        public Transform dhvCameraTransformHandler;

        InputProvider _inputProvider;
        float MOVE_CAMERA_SPEED_M_PER_SECOND = 3.5f;

        void Awake()
        {
            _inputProvider = new InputProvider();
        }

        void Update()
        {
            UpdatePosition();
        }

        void UpdatePosition()
        {
            Vector2 inputValue = _inputProvider.MovementInput().normalized;
            PerformMovement(inputValue);
        }

        void PerformMovement(Vector2 direction)
        {
            Vector3 directionIn3D = new Vector3(direction.x, 0, direction.y);
            Vector3 normalizedDirection = Vector3.Normalize(directionIn3D);
            dhvCameraTransformHandler.position = dhvCameraTransformHandler.position
                + normalizedDirection * MOVE_CAMERA_SPEED_M_PER_SECOND * Time.deltaTime;
        }

    }
}
