using BlobRPG.MainComponents;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Entities
{
    public class Fog
    {
        public vec3 FogColor { get => Settings.SkyColor; }
        public float Density { get; set; } = 0.0035f;
        public float Gradient { get; set; } = 5.0f;

        public Fog() { }
    }
}
