using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ReupVirtualTwin.webRequestersInterfaces;
using System.Threading.Tasks;

namespace ReupVirtualTwin.webRequesters
{
    public class TextureDownloader : ITextureDownloader
    {
        public async Task<Texture2D> DownloadTextureFromUrl(string url)
        {
            throw new System.NotImplementedException();
        }

    }
}
