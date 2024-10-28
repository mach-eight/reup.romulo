using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.controllerInterfaces
{
    public interface IZoomPositionRotationDHVController
    {
        Ray focusRay { set; }
        Ray startingFocusRay { set; }
        bool moveInDirection(Vector2 direction, float speed);
    }

}