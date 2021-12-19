using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.WavefrontOBJ.Models
{
    public class ModelData
    {
        public float[] Vertices { get; private set; }
        public float[] TextureCoords { get; private set; }
        public float[] Normals { get; private set; }
        public float[] Tangents { get; private set; }

        public int[] Indices { get; private set; }

        public float FurthestPoint { get; private set; }

        public ModelData(float[] vertices, float[] textureCoords, float[] normals, float[] tangents, int[] indices, float furthestPoint)
        {
            Vertices = vertices;
            TextureCoords = textureCoords;
            Normals = normals;
            Tangents = tangents;
            Indices = indices;
            FurthestPoint = furthestPoint;
        }
    }
}
