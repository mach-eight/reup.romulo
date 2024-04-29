using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

using ReupVirtualTwin.behaviourInterfaces;

namespace ReupVirtualTwin.behaviours
{
    public class WebRequestTexture : MonoBehaviour, IWebRequestTexture
    {
        public IEnumerator GetTexture(string url, Action<Texture2D> onSuccess, Action<string> onError)
        {
            var www = UnityWebRequestTexture.GetTexture(url);
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke(www.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                onSuccess?.Invoke(texture);
            }
        }
    }
}
