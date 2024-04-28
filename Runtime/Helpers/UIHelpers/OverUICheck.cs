using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace ReupVirtualTwin.helpers
{
    public static class OverUICheck
    {
        public static bool PointerOverUI()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);

            // Use the correct input method for WebGL
            if (Input.touchSupported && Input.touchCount > 0)
            {
                eventData.position = Input.GetTouch(0).position;
            }
            else
            {
                eventData.position = Input.mousePosition;
            }

            List<RaycastResult> raycastResultsList = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResultsList);

            // Check if the UI element is hit
            for (int i = 0; i < raycastResultsList.Count; i++)
            {
                if (raycastResultsList[i].gameObject != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
