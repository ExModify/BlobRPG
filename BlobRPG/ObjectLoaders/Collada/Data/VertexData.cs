using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.ObjectLoaders.Collada.Data
{
    public class VertexData
    {
		private const int NO_INDEX = -1;

		public vec3 Position { get; private set; }
		public int TextureIndex { get; set; }
		public int NormalIndex { get; set; }
		public VertexData DuplicateVertex { get; set; }
		public int Index { get; private set; }
		public float Length { get; private set; }
		public List<vec3> Tangents { get; private set; }
		public vec3 AveragedTangent { get; private set; }

		public VertexSkinData WeightsData { get; private set; }

		public bool Set
        {
            get
            {
				return TextureIndex != NO_INDEX && NormalIndex != NO_INDEX;
			}
        }

		public VertexData(int index, vec3 position, VertexSkinData weightsData)
		{
			Index = index;
			WeightsData = weightsData;
			Position = position;
			Length = position.Length;

			Tangents = new List<vec3>();
			AveragedTangent = vec3.Zero;
			TextureIndex = NO_INDEX;
			NormalIndex = NO_INDEX;
			DuplicateVertex = null;
		}

		public void AddTangent(vec3 tangent)
		{
			Tangents.Add(tangent);
		}

		public void AverageTangents()
		{
			if (Tangents.Count == 0)
			{
				return;
			}
			foreach (vec3 tangent in Tangents)
			{
				AveragedTangent += tangent;
			}
			AveragedTangent = AveragedTangent.Normalized;
		}

		public bool HasSameTextureAndNormal(int textureIndexOther, int normalIndexOther)
		{
			return textureIndexOther == TextureIndex && normalIndexOther == NormalIndex;
		}
	}
}
