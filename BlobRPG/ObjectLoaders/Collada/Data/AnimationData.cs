using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.ObjectLoaders.Collada.Data
{
	public class AnimationData
	{
		public float LengthSeconds { get; private set; }
		public KeyFrameData[] Frames { get; private set; }

		public AnimationData(float lengthSeconds, KeyFrameData[] frames)
		{
			LengthSeconds = lengthSeconds;
			Frames = frames;
		}

	}
}
