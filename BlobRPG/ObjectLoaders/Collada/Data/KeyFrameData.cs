using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.ObjectLoaders.Collada.Data
{
    public class KeyFrameData
	{
		public float Time { get; private set; }
		public List<JointTransformData> JointTransforms { get; private set; }

		public KeyFrameData(float time)
		{
			Time = time;
			JointTransforms = new List<JointTransformData>();
		}

		public void AddJointTransform(JointTransformData transform)
		{
			JointTransforms.Add(transform);
		}

	}
}
