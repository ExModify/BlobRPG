using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.ObjectLoaders.Collada.Data
{
    public class SkeletonData
	{
		public int JointCount { get; private set; }
		public JointData HeadJoint { get; private set; }
	
		public SkeletonData(int jointCount, JointData headJoint)
		{
			JointCount = jointCount;
			HeadJoint = headJoint;
		}

	}
}
