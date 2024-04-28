using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public interface IObjectSelector
    {
        public GameObject GetObject(Ray ray);
    }
}