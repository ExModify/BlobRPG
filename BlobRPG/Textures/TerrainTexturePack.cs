using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Textures
{
    public class TerrainTexturePack
    {
        public TerrainTexture BackgroundTexture { get; private set; }

        public TerrainTexture RTexture { get; private set; }
        public TerrainTexture GTexture { get; private set; }
        public TerrainTexture BTexture { get; private set; }

        public TerrainTexturePack(TerrainTexture backgroundTexture, TerrainTexture rTexture, TerrainTexture gTexture, TerrainTexture bTexture)
        {
            BackgroundTexture = backgroundTexture;
            RTexture = rTexture;
            GTexture = gTexture;
            BTexture = bTexture;
        }
    }
}
