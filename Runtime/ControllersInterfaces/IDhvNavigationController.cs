using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.controllerInterfaces
{
    public interface IDhvNavigationController
    {
        public bool isZooming { get; }
        public bool isRotating { get; }

        public void Rotate();
        public void Zoom();
        public void StopNavigationAction();
    }
}
