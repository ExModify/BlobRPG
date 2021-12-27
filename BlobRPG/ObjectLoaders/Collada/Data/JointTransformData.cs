using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.ObjectLoaders.Collada.Data
{
    public class JointTransformData
	{
		public String JointNameId { get; private set; }
		public mat4 JointLocalTransform { get; private set; }

		public JointTransformData(string jointNameId, mat4 jointLocalTransform)
		{
			JointNameId = jointNameId;
			JointLocalTransform = jointLocalTransform;
		}
	}
}
