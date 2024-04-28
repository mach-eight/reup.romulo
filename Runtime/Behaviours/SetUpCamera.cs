using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.behaviours
{
    public class SetUpCamera : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        void Start()
        {
            var cameras = Camera.allCameras;

            foreach (var cam in cameras)
            {
                var camComponent = cam.GetComponent<Camera>();
                camComponent.enabled = false;
            }
            _camera.enabled = true;
        }
    }
}