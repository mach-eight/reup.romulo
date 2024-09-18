using ReupVirtualTwin.inputs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateDhvCameraKeyboard : MonoBehaviour
{
    public Transform dhvCameraTransformHandler;

    InputProvider _inputProvider;
    const float ROTATE_SPEED = 40f;
    const float MAX_PITCH = 89f;
    const float MIN_PITCH = -89f;

    private float currentPitch = 0f;
    private float currentYaw = 0f;
    private float currentRoll = 0f;

    void Awake()
    {
        _inputProvider = new InputProvider();

        Vector3 initialRotation = dhvCameraTransformHandler.localRotation.eulerAngles;
        currentPitch = initialRotation.x;
        currentYaw = initialRotation.y;
        currentRoll = initialRotation.z;
    }

    void Update()
    {
        UpdateRotation();
    }

    void UpdateRotation()
    {
        Vector2 rotationInput = _inputProvider.DhvRotationInput();
        if (rotationInput != Vector2.zero)
        {
            ApplyRotation(rotationInput);
        }
    }

    void ApplyRotation(Vector2 rotationInput)
    {
        currentPitch = Mathf.Clamp(currentPitch + rotationInput.y * ROTATE_SPEED * Time.deltaTime, MIN_PITCH, MAX_PITCH);
        dhvCameraTransformHandler.localRotation = Quaternion.Euler(currentPitch, currentYaw, currentRoll);
    }
}
