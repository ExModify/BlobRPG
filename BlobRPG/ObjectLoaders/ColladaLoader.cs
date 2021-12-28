using BlobRPG.AnimationComponents.Objects;
using BlobRPG.AnimationComponents.Utils;
using BlobRPG.ObjectLoaders.Collada.Data;
using BlobRPG.ObjectLoaders.Collada.Loaders;
using BlobRPG.ObjectLoaders.Xml;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.ObjectLoaders
{
    public class ColladaLoader
	{
		public static AnimatedModelData LoadColladaModel(string colladaFile, int maxWeights)
        {
			FileStream fs = new FileStream(colladaFile, FileMode.Open);
			AnimatedModelData data = LoadColladaModel(fs, maxWeights);
			fs.Close();
			return data;

		}
		public static AnimatedModelData LoadColladaModel(Stream colladaFile, int maxWeights)
		{
			XmlNode node = XmlParser.LoadXML(colladaFile);

			SkinLoader skinLoader = new SkinLoader(node.GetChild("library_controllers"), maxWeights);
			SkinningData skinningData = skinLoader.ExtractSkinData();

			SkeletonLoader jointsLoader = new SkeletonLoader(node.GetChild("library_visual_scenes"), skinningData.JointOrder);
			SkeletonData jointsData = jointsLoader.ExtractBoneData();

			GeometryLoader g = new GeometryLoader(node.GetChild("library_geometries"), skinningData.VerticesSkinData);
			MeshData meshData = g.ExtractModelData();

			return new AnimatedModelData(meshData, jointsData);
		}
		public static Animation LoadColladaAnimation(string colladaFile)
		{
			FileStream fs = new FileStream(colladaFile, FileMode.Open);
			Animation data = LoadColladaAnimation(fs);
			fs.Close();
			return data;
		}
		public static Animation LoadColladaAnimation(Stream colladaFile)
		{
			AnimationData animationData = LoadColladaAnimationData(colladaFile);
			KeyFrame[] frames = new KeyFrame[animationData.Frames.Length];
			for (int i = 0; i < frames.Length; i++)
			{
				frames[i] = CreateKeyFrame(animationData.Frames[i]);
			}
			return new Animation(animationData.LengthSeconds, frames);
		}

		public static AnimationData LoadColladaAnimationData(string colladaFile)
		{
			FileStream fs = new FileStream(colladaFile, FileMode.Open);
			AnimationData data = LoadColladaAnimationData(fs);
			fs.Close();
			return data;

		}
		public static AnimationData LoadColladaAnimationData(Stream colladaFile)
		{

			XmlNode node = XmlParser.LoadXML(colladaFile);
			XmlNode animNode = node.GetChild("library_animations");
			XmlNode jointsNode = node.GetChild("library_visual_scenes");
			AnimationLoader loader = new AnimationLoader(animNode, jointsNode);
			AnimationData animData = loader.ExtractAnimation();
			return animData;
		}

		private static KeyFrame CreateKeyFrame(KeyFrameData data)
		{
			Dictionary<string, JointTransform> map = new();
			foreach (JointTransformData jointData in data.JointTransforms)
			{
				JointTransform jointTransform = CreateTransform(jointData);
				map.Add(jointData.JointNameId, jointTransform);
			}
			return new KeyFrame(data.Time, map);
		}

		private static JointTransform CreateTransform(JointTransformData data)
		{
			return new JointTransform(data.JointLocalTransform);
		}
	}
}
