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
        private AnimatedModel _Model = null;
        public AnimatedModel AnimatedModel
        {
            get
            {
                return _Model;
            }
            set
            {
                _Model = value;
                Animated = true;
            }
        }
        public bool Animated { get; private set; }

        public TexturedModel(RawModel model, ModelTexture texture)
        {
            Model = model;
            Texture = texture;
        }
    }
}
