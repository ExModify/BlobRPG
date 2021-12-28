using BlobRPG.Models;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Entities
{
    public class Entity
    {
        public TexturedModel Model { get; private set; }
        public vec3 Position { get; protected set; }
        public float RotationX { get; private set; }
        public float RotationY { get; private set; }
        public float RotationZ { get; private set; }
        public float Scale { get; private set; }

        public int TextureIndex { get; private set; }

        private AnimatedModel AnimatedModel { get; set; } = null;

        public Entity(TexturedModel model, vec3 position, float rx = 0, float ry = 0, float rz = 0, float scale = 1, int textureIndex = 0)
        {
            Model = model;
            if (model is AnimatedModel anim)
                AnimatedModel = anim;
            Position = position;
            RotationX = rx;
            RotationY = ry;
            RotationZ = rz;
            Scale = scale;

            TextureIndex = textureIndex;
        }

        public void Update()
        {
            AnimatedModel?.Update();
        }

        public void Move(float dx, float dy, float dz)
        {
            Position = new vec3(Position.x + dx, Position.y + dy, Position.z + dz);
        }
        public void Rotate(float dx, float dy, float dz)
        {
            RotationX += dx;
            RotationY += dy;
            RotationZ += dz;
        }

        public vec2 CalculateTextureOffset()
        {
            float column = TextureIndex % Model.Texture.NumberOfRows;
            float row = TextureIndex / Model.Texture.NumberOfRows;

            return new vec2(column / Model.Texture.NumberOfRows, row / Model.Texture.NumberOfRows);
        }
    }
}
