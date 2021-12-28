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
    public class SkeletonLoader
	{
		private static readonly mat4 Correction = mat4.Identity * mat4.RotateX((float)MathHelper.DegreesToRadians(-90));

		private readonly XmlNode ArmatureData;
		private readonly List<string> BoneOrder;
		private int JointCount = 0;

		public SkeletonLoader(XmlNode visualSceneNode, List<string> boneOrder)
		{
			ArmatureData = visualSceneNode.GetChild("visual_scene").GetChildWithAttribute("node", "id", "Armature");
			BoneOrder = boneOrder;
		}

		public SkeletonData ExtractBoneData()
		{
			XmlNode headNode = ArmatureData.GetChild("node");
			JointData headJoint = LoadJointData(headNode, true);
			return new SkeletonData(JointCount, headJoint);
		}

		private JointData LoadJointData(XmlNode jointNode, bool isRoot)
		{
			JointData joint = ExtractMainJointData(jointNode, isRoot);
			foreach (XmlNode childNode in jointNode.GetChildren("node"))
			{
				joint.AddChild(LoadJointData(childNode, false));
			}
			return joint;
		}

		private JointData ExtractMainJointData(XmlNode jointNode, bool isRoot)
		{
			string[] nameId = jointNode.GetAttribute("id").Split("_");
			string bone, id;
			if (nameId.Length != 2)
            {
				bone = id = jointNode.GetAttribute("id");
            }
            else
			{
				//bone = id = jointNode.GetAttribute("id");
				bone = nameId[1];
				id = nameId[0];
			}
			int index = BoneOrder.IndexOf(bone);
			string[] matrixData = jointNode.GetChild("matrix").Data.Split(" ");
			mat4 matrix = ConvertData(matrixData);
			Console.WriteLine(Tools.MatrixMaths.MatToString(matrix));
			if (isRoot)
			{
				matrix = Correction * matrix;
			}
			JointCount++;
			return new JointData(index, id, matrix);
		}

		private static mat4 ConvertData(string[] rawData)
		{
			mat4 mat = mat4.Identity;
			for (int i = 0; i < 16; i++)
			{
				mat[i] = float.Parse(rawData[i]);
			}
			return mat.Transposed;
		}
	}
}
