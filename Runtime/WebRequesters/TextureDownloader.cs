using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ReupVirtualTwin.webRequestersInterfaces;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace ReupVirtualTwin.webRequesters
{
    public class TextureDownloader : ITextureDownloader
    {
        public async Task<Texture2D> DownloadTextureFromUrl(string url)
        {
            using UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            await request.SendWebRequestTask();
            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new System.Exception($"Error: {request.error} for url: {url}");
            }
            return DownloadHandlerTexture.GetContent(request);
        }
    }
}
