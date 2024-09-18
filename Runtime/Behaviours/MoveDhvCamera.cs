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
        float MOVE_CAMERA_SPEED_M_PER_SECOND = 10f;

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
            Vector2 inputValue = _inputProvider.DhvMovementInput().normalized;
            PerformMovement(inputValue);
        }

        void PerformMovement(Vector2 direction)
        {
            Vector3 movementVector = new Vector3(direction.x, 0, direction.y);
            Vector3 cameraForward = Vector3.ProjectOnPlane(dhvCameraTransformHandler.forward, Vector3.up).normalized;
            Vector3 cameraRight = Vector3.Cross(Vector3.up, cameraForward).normalized;
            Vector3 finalMovement = cameraRight * movementVector.x + cameraForward * movementVector.z;
            Vector3 normalizedDirection = Vector3.Normalize(finalMovement);
            float movementDistance = MOVE_CAMERA_SPEED_M_PER_SECOND * Time.deltaTime;
            dhvCameraTransformHandler.position += normalizedDirection * movementDistance;
        }

    }
}
