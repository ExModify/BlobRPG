using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.ObjectLoaders.Collada.Data
{
	public class AnimatedModelData
	{
		public SkeletonData Joints { get; private set; }
		public MeshData Mesh { get; private set; }

		public AnimatedModelData(MeshData mesh, SkeletonData joints)
		{
			Joints = joints;
			Mesh = mesh;
		}
	}

}
