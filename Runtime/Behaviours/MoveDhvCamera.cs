using ReupVirtualTwin.helpers;
using ReupVirtualTwin.inputs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.behaviours
{
    public class MoveDhvCamera : MonoBehaviour
    {
        public Transform dollhouseViewWrapperTransform;

        InputProvider _inputProvider;
        public static readonly float KEYBOARD_MOVE_CAMERA_SPEED_METERS_PER_SECOND = 10;
        public static readonly float POINTER_MOVE_CAMERA_DISTANCE_IN_METERS_SQUARE_VIEWPORT = 10;

        float distancePerPixel;

        void Awake()
        {
            _inputProvider = new InputProvider();
            int pixelsInSquareViewport = ViewportUtils.MinViewportDimension(Camera.main);
            distancePerPixel = POINTER_MOVE_CAMERA_DISTANCE_IN_METERS_SQUARE_VIEWPORT / pixelsInSquareViewport;
        }

        void Update()
        {
            KeyboardUpdatePosition();
            PointerUpdatePosition();
        }

        void KeyboardUpdatePosition()
        {
            Vector2 inputValue = _inputProvider.KeyboardMoveDhvCamera().normalized;
            if (inputValue == Vector2.zero)
            {
                return;
            }
            Vector3 cameraForward = Vector3.ProjectOnPlane(dollhouseViewWrapperTransform.forward, Vector3.up).normalized;
            Vector3 cameraRight = Vector3.Cross(Vector3.up, cameraForward).normalized;
            Vector3 finalMovement = cameraRight * inputValue.x + cameraForward * inputValue.y;
            Vector3 normalizedDirection = Vector3.Normalize(finalMovement);
            float movementDistance = KEYBOARD_MOVE_CAMERA_SPEED_METERS_PER_SECOND * Time.deltaTime;
            dollhouseViewWrapperTransform.position += normalizedDirection * movementDistance;
        }

        void PointerUpdatePosition()
        {
            Vector2 inputValue = _inputProvider.PointerMoveDhvCamera();
            if (inputValue == Vector2.zero)
            {
                return;
            }
            Vector3 cameraForward = Vector3.ProjectOnPlane(dollhouseViewWrapperTransform.forward, Vector3.up).normalized;
            Vector3 cameraRight = Vector3.Cross(Vector3.up, cameraForward).normalized;
            float sideMovement = inputValue.x * distancePerPixel;
            float forwardMovement = inputValue.y * distancePerPixel;
            Vector3 movement = cameraRight * sideMovement + cameraForward * forwardMovement;
            dollhouseViewWrapperTransform.position += movement;

        }

    }
}
