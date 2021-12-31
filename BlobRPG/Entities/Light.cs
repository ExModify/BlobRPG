using GlmSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Entities
{
    public class Light
    {
        public vec3 Position { get; set; }
        public vec3 Color { get; set; }
        public vec3 Attenuation { get; set; } = new vec3(1, 0, 0);

        public Light(vec3 position, vec3 color, vec3 attenuation = default)
        {
            Position = position;
            Color = color;

            if (attenuation != default) Attenuation = attenuation;
        }
    }
}
