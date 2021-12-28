using BlobRPG.ObjectLoaders.Collada.Data;
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
    public class AnimationLoader
	{
		private static readonly mat4 Correction = mat4.Identity/* * mat4.RotateX((float)MathHelper.DegreesToRadians(-180))*/;
	
		private readonly XmlNode AnimationData;
		private readonly XmlNode JointHierarchy;

		public AnimationLoader(XmlNode animationData, XmlNode jointHierarchy)
		{
			AnimationData = animationData;
			JointHierarchy = jointHierarchy;
		}

		public AnimationData ExtractAnimation()
		{
			string rootNode = FindRootJointName();
			float[] times = GetKeyTimes();
			float duration = times[^1];
			KeyFrameData[] keyFrames = InitKeyFrames(times);
			List<XmlNode> animationNodes = AnimationData.GetChild("animation").GetChildren("animation");
			if (animationNodes.Count == 0)
            {
				animationNodes = AnimationData.GetChildren("animation");
			}
			foreach (XmlNode jointNode in animationNodes)
			{
				LoadJointTransforms(keyFrames, jointNode, rootNode);
			}
			return new AnimationData(duration, keyFrames);
		}

		private float[] GetKeyTimes()
		{
			XmlNode anim = AnimationData.GetChild("animation");
			XmlNode src = anim.GetChild("source");
			if (src == null)
            {
				src = anim.GetChild("animation").GetChild("source");
            }
			XmlNode timeData = src.GetChild("float_array");
			string[] rawTimes = timeData.Data.Split(" ");
			float[] times = new float[rawTimes.Length];
			for (int i = 0; i < times.Length; i++)
			{
				times[i] = float.Parse(rawTimes[i]);
			}
			return times;
		}

		private static KeyFrameData[] InitKeyFrames(float[] times)
		{
			KeyFrameData[] frames = new KeyFrameData[times.Length];
			for (int i = 0; i < frames.Length; i++)
			{
				frames[i] = new KeyFrameData(times[i]);
			}
			return frames;
		}

		private static void LoadJointTransforms(KeyFrameData[] frames, XmlNode jointData, string rootNodeId)
		{
			string jointNameId = GetJointName(jointData);
			string dataId = GetDataId(jointData);
			XmlNode transformData = jointData.GetChildWithAttribute("source", "id", dataId);
			string[] rawData = transformData.GetChild("float_array").Data.Split(" ");
			ProcessTransforms(jointNameId, rawData, frames, jointNameId == rootNodeId);
		}

		private static string GetDataId(XmlNode jointData)
		{
			XmlNode node = jointData.GetChild("sampler").GetChildWithAttribute("input", "semantic", "OUTPUT");
			return node.GetAttribute("source").Substring(1);
		}

		private static string GetJointName(XmlNode jointData)
		{
			XmlNode channelNode = jointData.GetChild("channel");
			string data = channelNode.GetAttribute("target");
			return data.Split("/")[0];
		}

		private static void ProcessTransforms(string jointName, string[] rawData, KeyFrameData[] keyFrames, bool root)
		{
			for (int i = 0; i < keyFrames.Length; i++)
			{
				mat4 transform = mat4.Identity;
				for (int j = 0; j < 16; j++)
				{
					transform[j] = float.Parse(rawData[i * 16 + j]);
				}
				transform = transform.Transposed;
				if (root)
				{
					transform = Correction * transform;
				}
				keyFrames[i].AddJointTransform(new JointTransformData(jointName, transform));
			}
		}

		private string FindRootJointName()
		{
			XmlNode skeleton = JointHierarchy.GetChild("visual_scene").GetChildWithAttribute("node", "id", "Armature");
			return skeleton.GetChild("node").GetAttribute("id");
		}
	}
}
