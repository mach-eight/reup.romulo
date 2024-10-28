using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.controllerInterfaces
{
    public interface IZoomPositionRotationDHVController
    {
        Vector2 focusScreenPoint { set; }
        Vector2 startingFocusScreenPoint { set; }
        bool moveInDirection(Vector2 direction, float speed);
    }

}