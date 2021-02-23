using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Textures
{
    public class TerrainTexture
    {
        public int TextureId { get; private set; }
        public TerrainTexture(int textureId)
        {
            TextureId = textureId;
        }
    }
}
