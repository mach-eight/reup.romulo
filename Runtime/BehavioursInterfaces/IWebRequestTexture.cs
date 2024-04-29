using System;
using System.Collections;
using UnityEngine;

namespace ReupVirtualTwin.behaviourInterfaces
{
    public interface IWebRequestTexture
    {
        IEnumerator GetTexture(string url, Action<Texture2D> onSuccess, Action<string> onError);
    }
}
