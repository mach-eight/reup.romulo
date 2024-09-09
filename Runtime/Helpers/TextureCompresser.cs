using UnityEngine;
using ReupVirtualTwin.helperInterfaces;

namespace ReupVirtualTwin.helpers
{
    public class TextureCompresser : ITextureCompresser
    {
        public Texture2D GetASTC12x12CompressedTexture(Texture2D texture)
        {
            Texture2D compressedTexture = new Texture2D(texture.width, texture.height, TextureFormat.ASTC_12x12, false);
            byte[] textureBytes = texture.EncodeToPNG();
            ImageConversion.LoadImage(compressedTexture, textureBytes);
            return compressedTexture;
        }
    }
}
