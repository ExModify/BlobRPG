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
        public vec3 Position { get; private set; }
        public float RotationX { get; private set; }
        public float RotationY { get; private set; }
        public float RotationZ { get; private set; }
        public float Scale { get; private set; }

        public Entity(TexturedModel model, vec3 position, float rx = 0, float ry = 0, float rz = 0, float scale = 1)
        {
            Model = model;
            Position = position;
            RotationX = rx;
            RotationY = ry;
            RotationZ = rz;
            Scale = scale;
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
    }
}
