using BlobRPG.ObjectLoaders.Collada.Data;
using BlobRPG.ObjectLoaders.Collada.Loaders;
using BlobRPG.ObjectLoaders.Xml;
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

		public static AnimationData LoadColladaAnimation(Stream colladaFile)
		{
			XmlNode node = XmlParser.LoadXML(colladaFile);
			XmlNode animNode = node.GetChild("library_animations");
			XmlNode jointsNode = node.GetChild("library_visual_scenes");
			AnimationLoader loader = new AnimationLoader(animNode, jointsNode);
			AnimationData animData = loader.ExtractAnimation();
			return animData;
		}
	}
}
