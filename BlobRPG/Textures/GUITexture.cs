using BlobRPG.Tools;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Textures
{
    public class GUITexture
    {
        private vec2 _position;
        private vec2 _scale;

        public int TextureId { get; private set; }

        public vec2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                RecalculateTransformationMatrix();
            }
        }
        public vec2 Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                _scale = value;
                RecalculateTransformationMatrix();
            }
        }

        public mat4 TransformationMatrix { get; set; }

        public GUITexture(int textureId, vec2 position = new vec2(), vec2 scale = new vec2())
        {
            TextureId = textureId;
            Position = position;
            Scale = scale;
        }

        private void RecalculateTransformationMatrix()
        {
            TransformationMatrix = MatrixMaths.CreateTransformationMatrix(_position, _scale);
        }
    }
}
