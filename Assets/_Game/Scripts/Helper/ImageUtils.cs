using System.IO;
using UnityEngine;
namespace _Game.Scripts.Helper
{
    public static class ImageUtils
    {
        public static byte[] SpriteToBytes(Sprite sprite)
        {
            if (sprite == null) return null;
            Texture2D texture = sprite.texture;
            if (texture.format != TextureFormat.RGBA32)
            {
                Texture2D newTexture = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
                newTexture.SetPixels(texture.GetPixels());
                newTexture.Apply();
                texture = newTexture;
            }
            return texture.EncodeToPNG();
        }
    }
}
