using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.ObjectLoaders.Collada.Data
{
    public class MeshData
	{
		public float[] Vertices { get; private set; }
		public float[] TextureCoords { get; private set; }
		public float[] Normals { get; private set; }
		public int[] Indices { get; private set; }
		public int[] JointIds { get; private set; }
		public float[] VertexWeights { get; private set; }

		public MeshData(float[] vertices, float[] textureCoords, float[] normals, int[] indices, int[] jointIds, float[] vertexWeights)
		{
			Vertices = vertices;
			TextureCoords = textureCoords;
			Normals = normals;
			Indices = indices;
			JointIds = jointIds;
			VertexWeights = vertexWeights;
		}
	}
}
