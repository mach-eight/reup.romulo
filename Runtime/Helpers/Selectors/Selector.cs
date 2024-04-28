using System;
using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public abstract class Selector : MonoBehaviour
    {
        [SerializeField]
        LayerMask ignoreLayerMask;

        protected bool CastRay(Ray ray, out RaycastHit hit)
        {
            return Physics.Raycast(ray, out hit, Mathf.Infinity, ~ignoreLayerMask);
        }

        /// <summary>
        /// This method should be overrided in every non abstract implmentation of a Selector
        /// Returns true if obj is the object we want to return, false otherwise
        /// </summary>
        /// <param name="obj">object to check</param>
        /// <returns>True if obj is the object we want to return, false otherwise </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        protected virtual GameObject GetSelectedObjectFromHitObject(GameObject obj)
        {
            throw new Exception("IsSelectable not implemented");
        }
    }
}