using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Textures
{
    public class ModelTexture
    {
        public int Id { get; private set; }

        public ModelTexture(int textureId)
        {
            Id = textureId;
        }
    }
}
