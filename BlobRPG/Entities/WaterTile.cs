using GlmSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Entities
{
    public class WaterTile
    {
        public const float TILE_SIZE = 60;

        public vec4 ReflectionClipPlane { get; private set; }
        public vec4 RefractionClipPlane { get; private set; }

        public float X { get; private set; }
        public float Height { get; private set; }
        public float Z { get; private set; }

        public WaterTile(float centerX, float centerZ, float height)
        {
            X = centerX;
            Z = centerZ;
            Height = height;

            ReflectionClipPlane = new(0, 1, 0, -height + 0.5f);
            RefractionClipPlane = new(0, -1, 0, height);
        }
    }
}
