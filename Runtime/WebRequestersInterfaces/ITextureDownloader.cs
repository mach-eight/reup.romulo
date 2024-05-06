using System.Threading.Tasks;
using UnityEngine;

namespace ReupVirtualTwin.webRequestersInterfaces
{
    public interface ITextureDownloader
    {
        public Task<Texture2D> DownloadTextureFromUrl(string url);
    }
}
