using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public interface IObjectWrapper
    {
        GameObject wrapper { get; }
        List<GameObject> wrappedObjects { get; }
        GameObject WrapObject(GameObject obj);
        GameObject UnwrapObject(GameObject obj);
        GameObject WrapObjects(GameObject[] obj);
        void DeWrapAll();
    }
}
