using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Models
{
    public struct RawModel
    {
        public int VaoId { get; private set; }
        public int VertexCount { get; private set; }

        public RawModel(int vaoId, int vertexCount)
        {
            VaoId = vaoId;
            VertexCount = vertexCount;
        }
    }
}
