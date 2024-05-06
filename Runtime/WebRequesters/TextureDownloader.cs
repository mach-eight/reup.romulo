using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Networking;

using ReupVirtualTwin.webRequestersInterfaces;

namespace ReupVirtualTwin.webRequesters
{
    public class TextureDownloader : ITextureDownloader
    {
        public async Task<Texture2D> DownloadTextureFromUrl(string url)
        {
            using UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            WebRequestResult result = await request.SendWebRequestTask();
            if (result.IsSuccess)
            {
                return DownloadHandlerTexture.GetContent(request);
            }
            return null;
        }
    }
}
