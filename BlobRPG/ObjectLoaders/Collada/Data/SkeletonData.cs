using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.ObjectLoaders.Collada.Data
{
    public class SkeletonData
	{
		public int JointCount;
		public JointData HeadJoint;
	
		public SkeletonData(int jointCount, JointData headJoint)
		{
			JointCount = jointCount;
			HeadJoint = headJoint;
		}

	}
}
