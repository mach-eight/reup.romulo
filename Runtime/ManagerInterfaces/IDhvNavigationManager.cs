using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.controllerInterfaces
{
    public interface IDhvNavigationManager
    {
        public bool isZooming { get; }
        public bool isRotating { get; }

        public void Rotate();
        public void Zoom();
        public void StopRotation();
        public void StopZoom();
    }
}
