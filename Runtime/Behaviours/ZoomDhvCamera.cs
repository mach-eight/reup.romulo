using ReupVirtualTwin.inputs;
using UnityEngine;
using Unity.Cinemachine;

public class ZoomDhvCamera : MonoBehaviour
{
    public GameObject virtualCamera;
    private CinemachineCamera cam;
    InputProvider _inputProvider;
    [SerializeField] private float ZOOM_SPEED = 10f;
    [SerializeField] private float MIN_FOV = 20f;
    [SerializeField] private float MAX_FOV = 60f;

    void Awake()
    {
        _inputProvider = new InputProvider();
        cam = virtualCamera.GetComponent<CinemachineCamera>();
    }

    void Update()
    {
        UpdateFOV();
    }

    void UpdateFOV()
    {
        Vector2 inputValue = _inputProvider.DhvZoomInput().normalized;
        float zoomAmount = inputValue.y * ZOOM_SPEED * Time.deltaTime;

        float newFOV = cam.Lens.FieldOfView - zoomAmount;

        newFOV = Mathf.Clamp(newFOV, MIN_FOV, MAX_FOV);

        cam.Lens.FieldOfView = newFOV;
    }
}