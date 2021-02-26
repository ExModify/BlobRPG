﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Textures
{
    public class ModelTexture
    {
        public int Id { get; private set; }

        public float ShineDamper { get; set; } = 1;
        public float Reflectivity { get; set; } = 0;
        public bool HasTransparency { get; set; } = false;
        public bool UseFakeLighting { get; set; } = false;

        public int NumberOfRows { get; set; }

        public ModelTexture(int textureId, int numberOfRows = 1)
        {
            Id = textureId;
            NumberOfRows = numberOfRows;
        }
    }
}
