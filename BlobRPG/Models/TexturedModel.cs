using BlobRPG.Textures;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Models
{
    public class TexturedModel
    {
        public RawModel Model { get; private set; }
        public ModelTexture Texture { get; private set; }

        public TexturedModel(RawModel model, ModelTexture texture)
        {
            Model = model;
            Texture = texture;
        }
    }
}
