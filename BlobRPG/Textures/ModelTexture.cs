using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Textures
{
    public class ModelTexture
    {
        private int _SpecularMap;

        public int Id { get; private set; }
        public int NormalMap { get; set; }
        public int SpecularMap
        {
            get
            {
                return _SpecularMap;
            }
            set
            {
                _SpecularMap = value;
                HasSpecularMap = true;
            }
        }

        public float ShineDamper { get; set; } = 1;
        public float Reflectivity { get; set; } = 0;
        public bool HasTransparency { get; set; } = false;
        public bool UseFakeLighting { get; set; } = false;
        public bool HasSpecularMap { get; private set; } = false;

        public int NumberOfRows { get; set; }

        public ModelTexture(int textureId, int numberOfRows = 1)
        {
            Id = textureId;
            NumberOfRows = numberOfRows;
        }
        public void RemoveSpecularMap()
        {
            HasSpecularMap = false;
        }
    }
}
