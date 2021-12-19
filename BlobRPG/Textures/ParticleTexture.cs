using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Textures
{
    public class ParticleTexture
    {
        public int TextureId { get; set; }
        public int NumberOfRows { get; set; }

        public bool Additive { get; set; }

        public ParticleTexture(int textureId, int numberOfRows, bool additive = false)
        {
            TextureId = textureId;
            NumberOfRows = numberOfRows;
            Additive = additive;
        }
    }
}
