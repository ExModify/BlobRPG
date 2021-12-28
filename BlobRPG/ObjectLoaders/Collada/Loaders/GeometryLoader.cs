using BlobRPG.ObjectLoaders.Collada.Data;
using BlobRPG.ObjectLoaders.Models;
using BlobRPG.ObjectLoaders.Xml;
using GlmSharp;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.ObjectLoaders.Collada.Loaders
{
    class GeometryLoader
	{
		private static readonly mat4 Correction = mat4.Identity/* * mat4.RotateX((float)MathHelper.DegreesToRadians(-180))*/;

		private readonly XmlNode MeshData;

		private readonly List<VertexSkinData> VertexWeights;

		private float[] VerticesArray;
		private float[] NormalsArray;
		private float[] TexturesArray;
		private int[] IndicesArray;
		private int[] JointIdsArray;
		private float[] WeightsArray;
		private float[] TangentsArray;

		List<VertexData> Vertices;
		List<vec2> Textures;
		List<vec3> Normals;
		List<int> Indices;

		public GeometryLoader(XmlNode geometryNode, List<VertexSkinData> vertexWeights)
		{
			VertexWeights = vertexWeights;
			MeshData = geometryNode.GetChild("geometry").GetChild("mesh");

			Vertices = new List<VertexData>();
			Textures = new List<vec2>();
			Normals = new List<vec3>();
			Indices = new List<int>();
		}

		public MeshData ExtractModelData()
		{
			ReadRawData();
			AssembleVertices();
			RemoveUnusedVertices();
			ConvertDataToArrays();
			IndicesArray = Indices.ToArray();
			return new MeshData(VerticesArray, TexturesArray, NormalsArray, TangentsArray, IndicesArray, JointIdsArray, WeightsArray);
		}

		private void ReadRawData()
		{
			ReadPositions();
			ReadNormals();
			ReadTextureCoords();
		}

		private void ReadPositions()
		{
			string positionsId = MeshData.GetChild("vertices").GetChild("input").GetAttribute("source").Substring(1);
			XmlNode positionsData = MeshData.GetChildWithAttribute("source", "id", positionsId).GetChild("float_array");
			int count = int.Parse(positionsData.GetAttribute("count"));
			string[] posData = positionsData.Data.Split(" ");
			for (int i = 0; i < count / 3; i++)
			{
				float x = float.Parse(posData[i * 3]);
				float y = float.Parse(posData[i * 3 + 1]);
				float z = float.Parse(posData[i * 3 + 2]);
				vec4 position = new vec4(x, y, z, 1);
				position = Correction * position;
				Vertices.Add(new VertexData(Vertices.Count, new vec3(position.x, position.y, position.z), VertexWeights[Vertices.Count]));
			}
		}

		private void ReadNormals()
		{
			XmlNode baseNode = MeshData.GetChild("polylist");
			if (baseNode == null)
				baseNode = MeshData.GetChild("triangles");
			string normalsId = baseNode.GetChildWithAttribute("input", "semantic", "NORMAL").GetAttribute("source").Substring(1);
			XmlNode normalsData = MeshData.GetChildWithAttribute("source", "id", normalsId).GetChild("float_array");
			int count = int.Parse(normalsData.GetAttribute("count"));
			string[] normData = normalsData.Data.Split(" ");
			for (int i = 0; i < count / 3; i++)
			{
				float x = float.Parse(normData[i * 3]);
				float y = float.Parse(normData[i * 3 + 1]);
				float z = float.Parse(normData[i * 3 + 2]);
				vec4 norm = new vec4(x, y, z, 0f);
				norm = Correction * norm;;
				Normals.Add(new vec3(norm.x, norm.y, norm.z));
			}
		}

		private void ReadTextureCoords()
		{
			XmlNode baseNode = MeshData.GetChild("polylist");
			if (baseNode == null)
				baseNode = MeshData.GetChild("triangles");
			string texCoordsId = baseNode.GetChildWithAttribute("input", "semantic", "TEXCOORD").GetAttribute("source").Substring(1);
			XmlNode texCoordsData = MeshData.GetChildWithAttribute("source", "id", texCoordsId).GetChild("float_array");
			int count = int.Parse(texCoordsData.GetAttribute("count"));
			string[] texData = texCoordsData.Data.Split(" ");
			for (int i = 0; i < count / 2; i++)
			{
				float s = float.Parse(texData[i * 2]);
				float t = float.Parse(texData[i * 2 + 1]);
				Textures.Add(new vec2(s, t));
			}
		}

		private void AssembleVertices()
		{
			XmlNode poly = MeshData.GetChild("polylist");
			if (poly == null)
				poly = MeshData.GetChild("triangles");
			int typeCount = poly.GetChildren("input").Count;
			string[] indexData = poly.GetChild("p").Data.Split(" ");
			for (int i = 0; i < indexData.Length / typeCount; i++)
			{
				int positionIndex = int.Parse(indexData[i * typeCount]);
				int normalIndex = int.Parse(indexData[i * typeCount + 1]);
				int texCoordIndex = int.Parse(indexData[i * typeCount + 2]);
				ProcessVertex(positionIndex, normalIndex, texCoordIndex);
			}
		}


		private VertexData ProcessVertex(int posIndex, int normIndex, int texIndex)
		{
			VertexData currentVertex = Vertices[posIndex];
			if (!currentVertex.Set)
			{
				currentVertex.TextureIndex = texIndex;
				currentVertex.NormalIndex = normIndex;
				Indices.Add(posIndex);
				return currentVertex;
			}
			else
			{
				return DealWithAlreadyProcessedVertex(currentVertex, texIndex, normIndex);
			}
		}

		private float ConvertDataToArrays()
		{
			VerticesArray = new float[Vertices.Count * 3];
			TexturesArray = new float[Vertices.Count * 2];
			NormalsArray = new float[Vertices.Count * 3];
			TangentsArray = new float[Vertices.Count * 3];
			JointIdsArray = new int[Vertices.Count * 3];
			WeightsArray = new float[Vertices.Count * 3];

			float furthestPoint = 0;
			for (int i = 0; i < Vertices.Count; i++)
			{
				VertexData currentVertex = Vertices[i];
				if (currentVertex.Length > furthestPoint)
				{
					furthestPoint = currentVertex.Length;
				}
				vec3 position = currentVertex.Position;
				vec2 textureCoord = Textures[currentVertex.TextureIndex];
				vec3 normalVector = Normals[currentVertex.NormalIndex];
				vec3 tangent = currentVertex.AveragedTangent;
				VerticesArray[i * 3] = position.x;
				VerticesArray[i * 3 + 1] = position.y;
				VerticesArray[i * 3 + 2] = position.z;
				TexturesArray[i * 2] = textureCoord.x;
				TexturesArray[i * 2 + 1] = 1 - textureCoord.y;
				NormalsArray[i * 3] = normalVector.x;
				NormalsArray[i * 3 + 1] = normalVector.y;
				NormalsArray[i * 3 + 2] = normalVector.z;
				VertexSkinData weights = currentVertex.WeightsData;
				JointIdsArray[i * 3] = weights.JointIds[0];
				JointIdsArray[i * 3 + 1] = weights.JointIds[1];
				JointIdsArray[i * 3 + 2] = weights.JointIds[2];
				WeightsArray[i * 3] = weights.Weights[0];
				WeightsArray[i * 3 + 1] = weights.Weights[1];
				WeightsArray[i * 3 + 2] = weights.Weights[2];
				TangentsArray[i * 3] = tangent.x;
				TangentsArray[i * 3 + 1] = tangent.y;
				TangentsArray[i * 3 + 2] = tangent.z;

			}
			return furthestPoint;
		}

		private VertexData DealWithAlreadyProcessedVertex(VertexData previousVertex, int newTextureIndex, int newNormalIndex)
		{
			if (previousVertex.HasSameTextureAndNormal(newTextureIndex, newNormalIndex))
			{
				Indices.Add(previousVertex.Index);
				return previousVertex;
			}
			else
			{
				VertexData anotherVertex = previousVertex.DuplicateVertex;
				if (anotherVertex != null)
				{
					return DealWithAlreadyProcessedVertex(anotherVertex, newTextureIndex, newNormalIndex);
				}
				else
				{
					VertexData duplicateVertex = new VertexData(Vertices.Count, previousVertex.Position, previousVertex.WeightsData);
					duplicateVertex.TextureIndex = newTextureIndex;
					duplicateVertex.NormalIndex = newNormalIndex;
					previousVertex.DuplicateVertex = duplicateVertex;
					Vertices.Add(duplicateVertex);
					Indices.Add(duplicateVertex.Index);
					return duplicateVertex;
				}

			}
		}

		private void RemoveUnusedVertices()
		{
			foreach (VertexData vertex in Vertices)
			{
				vertex.AverageTangents();
				if (!vertex.Set)
				{
					vertex.TextureIndex = 0;
					vertex.NormalIndex = 0;
				}
			}
		}
	}
}
