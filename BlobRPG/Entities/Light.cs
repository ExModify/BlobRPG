using GlmSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Entities
{
    public class Light
    {
        public vec3 Position { get; private set; }
        public vec3 Color { get; private set; }

        public Light(vec3 position, vec3 color)
        {
            Position = position;
            Color = color;
        }
    }
}
