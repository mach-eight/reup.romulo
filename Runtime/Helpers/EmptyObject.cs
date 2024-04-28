using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public class ObjectHelpers : MonoBehaviour
    {
        public static void DestroyChildren(GameObject parent)
        { 
            foreach(Transform child in parent.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}