using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.AnimationComponents.Objects
{
    public class KeyFrame
    {
        public float TimeStamp { get; private set; }
        public Dictionary<string, JointTransform> Pose { get; private set; }

        public KeyFrame(float timeStamp, Dictionary<string, JointTransform> jointKeyFrames)
        {
            TimeStamp = timeStamp;
            Pose = jointKeyFrames;
        }
    }
}
