using ReupVirtualTwin.inputs;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotateAndZoomDhvCameraTouch : MonoBehaviour
{
    public GameObject virtualCamera;
    [SerializeField] private float ZOOM_SPEED = 10f;
    [SerializeField] private float MIN_FOV = 20f;
    [SerializeField] private float MAX_FOV = 60f;
    [SerializeField] private float rotationSpeed = 0.5f;

    private CinemachineCamera cam;
    private Transform camTransform;
    InputProvider _inputProvider;
    private Vector2 startingPosition1;
    private Vector2 startingPosition2;
    private float startingDistance;
    private float startingAngle;

    private void OnEnable()
    {
        _inputProvider.dhvZoomRotationTouchPerformedT1 += OnFingerPositionPerformed;
        _inputProvider.dhvZoomRotationTouchPerformedT2 += OnFingerPositionPerformed;
    }

    private void OnDisable()
    {
        _inputProvider.dhvZoomRotationTouchPerformedT1 -= OnFingerPositionPerformed;
        _inputProvider.dhvZoomRotationTouchPerformedT2 -= OnFingerPositionPerformed;
    }

    void Awake()
    {
        _inputProvider = new InputProvider();
        cam = virtualCamera.GetComponent<CinemachineCamera>();
        camTransform = virtualCamera.transform;
    }

    private void OnFingerPositionPerformed(InputAction.CallbackContext context)
    {
        // Check if both fingers are touching the screen
        if (_inputProvider.DhvZoomRotationTouchT2InputPhase() == InputActionPhase.Performed)
        {
            Vector2 position1 = _inputProvider.DhvZoomRotationTouchT1Input();
            Vector2 position2 = _inputProvider.DhvZoomRotationTouchT2Input();

            if (startingDistance == 0)
            {
                // Initialize starting positions and distance
                startingPosition1 = position1;
                startingPosition2 = position2;
                startingDistance = Vector2.Distance(position1, position2);
                startingAngle = Mathf.Atan2(position2.y - position1.y, position2.x - position1.x);
            }
            else
            {
                // Pinch Zoom
                float currentDistance = Vector2.Distance(position1, position2);
                float delta = currentDistance - startingDistance;
                float zoomAmount = delta * ZOOM_SPEED * Time.deltaTime;
                float newFOV = cam.Lens.FieldOfView - zoomAmount;
                cam.Lens.FieldOfView = Mathf.Clamp(newFOV, MIN_FOV, MAX_FOV);

                // Rotation
                float currentAngle = Mathf.Atan2(position2.y - position1.y, position2.x - position1.x);
                float angleDifference = Mathf.DeltaAngle(startingAngle * Mathf.Rad2Deg, currentAngle * Mathf.Rad2Deg);
                camTransform.Rotate(Vector3.up, -angleDifference * rotationSpeed);

                // Update starting values for the next frame
                startingPosition1 = position1;
                startingPosition2 = position2;
                startingDistance = currentDistance;
                startingAngle = currentAngle;
            }
        }
        else
        {
            // Reset values when fingers are lifted
            startingDistance = 0;
            startingAngle = 0;
        }
    }
}
