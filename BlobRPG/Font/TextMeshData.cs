using BlobRPG.Models;
using GlmSharp;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Font
{
    public class TextMeshData
	{
		public float[] VertexPositions { get; private set; }
		public float[] TextureCoords { get; private set; }

		public TextMeshData(float[] vertexPositions, float[] textureCoords)
		{
			VertexPositions = vertexPositions;
			TextureCoords = textureCoords;
		}
	}
}
