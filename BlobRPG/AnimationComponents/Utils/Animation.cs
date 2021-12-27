using BlobRPG.AnimationComponents.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.AnimationComponents.Utils
{
    public class Animation
    {
        public float Length { get; private set; }
        public KeyFrame[] Frames { get; private set; }

        public Animation(float lengthInSeconds, KeyFrame[] frames)
        {
            Length = lengthInSeconds;
            Frames = frames;
        }
    }
}
