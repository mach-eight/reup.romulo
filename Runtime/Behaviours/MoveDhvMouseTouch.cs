using ReupVirtualTwin.inputs;
using ReupVirtualTwin.managers;
using UnityEngine;

public class MoveDhvMouseTouch : MonoBehaviour
{
    public Transform virtualCamera;
    public float sensitivity = 20f;
    public float smoothTime = 0.1f;
    public DragManager _dragManager;

    private InputProvider _inputProvider;
    private Vector3 currentVelocity = Vector3.zero;

    private void Awake()
    {
        _inputProvider = new InputProvider();
    }

    void Update()
    {
        if (_dragManager.dragging)
        {
            Vector2 look = _inputProvider.DhvMovementMouseAndTouch();
            PerformMovement(look);
        }
    }

    private void PerformMovement(Vector2 direction)
    {
        Vector3 cameraForward = Vector3.ProjectOnPlane(virtualCamera.forward, Vector3.up).normalized;
        Vector3 cameraRight = Vector3.Cross(Vector3.up, cameraForward).normalized;
        Vector3 targetMovement = (cameraRight * direction.x + cameraForward * direction.y).normalized;
        float movementDistance = sensitivity * Time.deltaTime;
        Vector3 targetPosition = virtualCamera.position + targetMovement * movementDistance;
        virtualCamera.position = Vector3.SmoothDamp(virtualCamera.position, targetPosition, ref currentVelocity, smoothTime);
    }
}
