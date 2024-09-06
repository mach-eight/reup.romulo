using UnityEngine;

namespace ReupVirtualTwin.helperInterfaces
{
    public interface ITextureCompresser
    {
        public Texture2D GetASTC12x12CompressedTexture(Texture2D texture);
    }
}
