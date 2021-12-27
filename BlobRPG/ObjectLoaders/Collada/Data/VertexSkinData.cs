using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.ObjectLoaders.Collada.Data
{
    public class VertexSkinData
	{
		public List<int> JointIds { get; private set; }
		public List<float> Weights { get; private set; }

		public VertexSkinData()
        {
			JointIds = new();
			Weights = new();
		}

		public void AddJointEffect(int jointId, float weight)
		{
			for (int i = 0; i < Weights.Count; i++)
			{
				if (weight > Weights[i])
				{
					JointIds.Insert(i, jointId);
					Weights.Insert(i, weight);
					return;
				}
			}
			JointIds.Add(jointId);
			Weights.Add(weight);
		}

		public void LimitJointNumber(int max)
		{
			if (JointIds.Count > max)
			{
				float[] topWeights = new float[max];
				float total = AaveTopWeights(topWeights);
				RefillWeightList(topWeights, total);
				RemoveExcessJointIds(max);
			}
			else if (JointIds.Count < max)
			{
				FillEmptyWeights(max);
			}
		}

		private void FillEmptyWeights(int max)
		{
			while (JointIds.Count < max)
			{
				JointIds.Add(0);
				Weights.Add(0f);
			}
		}

		private float AaveTopWeights(float[] topWeightsArray)
		{
			float total = 0;
			for (int i = 0; i < topWeightsArray.Length; i++)
			{
				topWeightsArray[i] = Weights[i];
				total += topWeightsArray[i];
			}
			return total;
		}

		private void RefillWeightList(float[] topWeights, float total)
		{
			Weights.Clear();
			for (int i = 0; i < topWeights.Length; i++)
			{
				Weights.Add(Math.Min(topWeights[i] / total, 1));
			}
		}

		private void RemoveExcessJointIds(int max)
		{
			while (JointIds.Count > max)
			{
				JointIds.RemoveAt(JointIds.Count - 1);
			}
		}

	}
}
