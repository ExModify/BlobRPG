using BlobRPG.AnimationComponents.Objects;
using BlobRPG.Models;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.AnimationComponents.Utils
{
    public class Animator
    {
        public AnimatedModel Entity { get; private set; }

        public Animation CurrentAnimation { get; private set; }
        public float AnimationTime { get; private set; } = 0;

        public Animator(AnimatedModel model)
        {
            Entity = model;
        }

        public void Animate(Animation animation)
        {
            AnimationTime = 0;
            CurrentAnimation = animation;
        }
        public void Update()
        {
            if (CurrentAnimation == null) return;
			IncreaseAnimationTime();
			Dictionary<string, mat4> currentPose = CalculateCurrentAnimationPose();
			mat4 refmat = mat4.Identity;
			ApplyPoseToJoints(currentPose, Entity.RootJoint, ref refmat);
		}

		private void IncreaseAnimationTime()
		{
			AnimationTime += (float)Settings.DeltaTime;
			if (AnimationTime > CurrentAnimation.Length)
			{
				AnimationTime %= CurrentAnimation.Length;
			}
		}
		private Dictionary<string, mat4> CalculateCurrentAnimationPose()
		{
			KeyFrame[] frames = GetPreviousAndNextFrames();
			float progression = CalculateProgression(frames[0], frames[1]);
			return InterpolatePoses(frames[0], frames[1], progression);
		}
		private void ApplyPoseToJoints(Dictionary<string, mat4> currentPose, Joint joint, ref mat4 parentTransform)
		{
			mat4 currentLocalTransform = currentPose[joint.Name];
			mat4 currentTransform = parentTransform * currentLocalTransform;
			foreach (Joint childJoint in joint.Children)
			{
				ApplyPoseToJoints(currentPose, childJoint, ref currentTransform);
			}
			currentTransform *= joint.InverseBindTransform;
			joint.AnimatedTransform = currentTransform;
		}

		private KeyFrame[] GetPreviousAndNextFrames()
		{
			KeyFrame[] allFrames = CurrentAnimation.Frames;
			KeyFrame previousFrame = allFrames[0];
			KeyFrame nextFrame = allFrames[0];
			for (int i = 1; i < allFrames.Length; i++)
			{
				nextFrame = allFrames[i];
				if (nextFrame.TimeStamp > AnimationTime)
				{
					break;
				}
				previousFrame = allFrames[i];
			}
			return new KeyFrame[] { previousFrame, nextFrame };
		}

		private float CalculateProgression(KeyFrame previousFrame, KeyFrame nextFrame)
		{
			float totalTime = nextFrame.TimeStamp - previousFrame.TimeStamp;
			float currentTime = AnimationTime - previousFrame.TimeStamp;
			return currentTime / totalTime;
		}

		private static Dictionary<string, mat4> InterpolatePoses(KeyFrame previousFrame, KeyFrame nextFrame, float progression)
		{
			Dictionary<string, mat4> currentPose = new();
			foreach (string jointName in previousFrame.Pose.Keys)
			{
				JointTransform previousTransform = previousFrame.Pose[jointName];
				JointTransform nextTransform = nextFrame.Pose[jointName];
				JointTransform currentTransform = JointTransform.Interpolate(previousTransform, nextTransform, progression);
				currentPose.Add(jointName, currentTransform.LocalTransform);
			}
			return currentPose;
		}
	}
}
