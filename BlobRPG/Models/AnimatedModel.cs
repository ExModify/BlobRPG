using BlobRPG.AnimationComponents.GLObjects;
using BlobRPG.AnimationComponents.Objects;
using BlobRPG.AnimationComponents.Utils;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Models
{
    public class AnimatedModel
    {
        public Vao Model { get; private set; }
        public Texture Texture { get; private set; }

        public Joint RootJoint { get; private set; }
        public int JointCount { get; private set; }

        public Animator Animator { get; private set; }

        public AnimatedModel(Vao model, Texture texture, Joint rootJoint, int jointCount)
        {
            Model = model;
            Texture = texture;
            RootJoint = rootJoint;
            JointCount = jointCount;
            Animator = new Animator(this);

            mat4 refmat = mat4.Identity;
            rootJoint.CalculateInverseBindTranform(ref refmat);
        }

        public void Delete()
        {
            Model.Delete();
            Texture.Delete();
        }
        public void Animate(Animation animation)
        {
            Animator.Animate(animation);
        }
        public void Update()
        {
            Animator.Update();
        }
        public mat4[] GetJointTransforms()
        {
            mat4[] jointMatrices = new mat4[JointCount];
            AddJointsToArray(RootJoint, jointMatrices);
            return jointMatrices;
        }
        private void AddJointsToArray(Joint headJoint, mat4[] jointMatrices)
        {
            jointMatrices[headJoint.Index] = headJoint.AnimatedTransform;
            foreach (Joint childJoint in headJoint.Children)
            {
                AddJointsToArray(childJoint, jointMatrices);
            }
        }
    }
}
