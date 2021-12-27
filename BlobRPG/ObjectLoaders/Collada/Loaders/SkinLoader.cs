using BlobRPG.ObjectLoaders.Collada.Data;
using BlobRPG.ObjectLoaders.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.ObjectLoaders.Collada.Loaders
{
    class SkinLoader
	{
		private readonly XmlNode SkinningData;
		private readonly int MaxWeights;

		public SkinLoader(XmlNode controllersNode, int maxWeights)
		{
			SkinningData = controllersNode.GetChild("controller").GetChild("skin");
			MaxWeights = maxWeights;
		}

		public SkinningData ExtractSkinData()
		{
			List<String> jointsList = LoadJointsList();
			float[] weights = LoadWeights();
			XmlNode weightsDataNode = SkinningData.GetChild("vertex_weights");
			int[] effectorJointCounts = GetEffectiveJointsCounts(weightsDataNode);
			List<VertexSkinData> vertexWeights = GetSkinData(weightsDataNode, effectorJointCounts, weights);
			return new SkinningData(jointsList, vertexWeights);
		}

		private List<string> LoadJointsList()
		{
			XmlNode inputNode = SkinningData.GetChild("vertex_weights");
			string jointDataId = inputNode.GetChildWithAttribute("input", "semantic", "JOINT").GetAttribute("source").Substring(1);
			XmlNode jointsNode = SkinningData.GetChildWithAttribute("source", "id", jointDataId).GetChild("Name_array");
			string[] names = jointsNode.Data.Split(" ");
			List<string> jointsList = new();
			foreach (string name in names)
			{
				jointsList.Add(name);
			}
			return jointsList;
		}

		private float[] LoadWeights()
		{
			XmlNode inputNode = SkinningData.GetChild("vertex_weights");
			string weightsDataId = inputNode.GetChildWithAttribute("input", "semantic", "WEIGHT").GetAttribute("source").Substring(1);
			XmlNode weightsNode = SkinningData.GetChildWithAttribute("source", "id", weightsDataId).GetChild("float_array");
			string[] rawData = weightsNode.Data.Split(" ");
			float[] weights = new float[rawData.Length];
			for (int i = 0; i < weights.Length; i++)
			{
				weights[i] = float.Parse(rawData[i]);
			}
			return weights;
		}

		private List<VertexSkinData> GetSkinData(XmlNode weightsDataNode, int[] counts, float[] weights)
		{
			String[] rawData = weightsDataNode.GetChild("v").Data.Split(" ");
			List<VertexSkinData> skinningData = new();
			int pointer = 0;
			foreach (int count in counts)
			{
				VertexSkinData skinData = new VertexSkinData();
				for (int i = 0; i < count; i++)
				{
					int jointId = int.Parse(rawData[pointer++]);
					int weightId = int.Parse(rawData[pointer++]);
					skinData.AddJointEffect(jointId, weights[weightId]);
				}
				skinData.LimitJointNumber(MaxWeights);
				skinningData.Add(skinData);
			}
			return skinningData;
		}
		private static int[] GetEffectiveJointsCounts(XmlNode weightsDataNode)
		{
			string[] rawData = weightsDataNode.GetChild("vcount").Data.Split(" ");
			int[] counts = new int[rawData.Length];
			for (int i = 0; i < rawData.Length; i++)
			{
				counts[i] = int.Parse(rawData[i]);
			}
			return counts;
		}

	}
}
