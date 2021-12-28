using BlobRPG.AnimationComponents.Objects;
using BlobRPG.AnimationComponents.Utils;
using BlobRPG.MainComponents;
using BlobRPG.ObjectLoaders;
using BlobRPG.ObjectLoaders.Collada.Data;
using BlobRPG.ObjectLoaders.Collada.Loaders;
using BlobRPG.Textures;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Models
{
    public class AnimatedModel : TexturedModel
    {
        public Joint RootJoint { get; private set; }
        public int JointCount { get; private set; }

        public Animator Animator { get; private set; }

        public AnimatedModel(RawModel model, ModelTexture texture, Joint rootJoint, int jointCount) : base(model, texture)
        {
            RootJoint = rootJoint;
            JointCount = jointCount;
            Animator = new Animator(this, false, false);

            mat4 refmat = mat4.Identity;
            rootJoint.CalculateInverseBindTranform(ref refmat);
            AnimatedModel = this;
        }
        public void BindActionToKeyframe(Action action, int frame)
        {
            Animator.AddAction(action, frame);
        }
        public void ActivateAndResetAnimation()
        {
            Animator.Active = true;
            Animator.Reset();
        }
        public void UseAnimation(Animation animation)
        {
            Animator.UseAnimation(animation);
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

        public static AnimatedModel LoadModel(string colladaFile, string textureFile, int shineDamper = 10, int reflectivity = 1, int numberOfRows = 2)
        {
            AnimatedModelData entityData = ColladaLoader.LoadColladaModel(colladaFile, Settings.MaxWeights);
            RawModel model = Loader.LoadToVao(entityData.Mesh.Vertices, entityData.Mesh.TextureCoords, entityData.Mesh.Normals, entityData.Mesh.Tangents, entityData.Mesh.JointIds, entityData.Mesh.VertexWeights, entityData.Mesh.Indices);
            
            ModelTexture texture = new ModelTexture(Loader.LoadTexture(textureFile))
            {
                ShineDamper = shineDamper,
                Reflectivity = reflectivity,
                NumberOfRows = numberOfRows
            };
            
            
            SkeletonData skeletonData = entityData.Joints;
            Joint headJoint = CreateJoints(skeletonData.HeadJoint);

            AnimatedModel m = new AnimatedModel(model, texture, headJoint, skeletonData.JointCount);
            Animation anim = ColladaLoader.LoadColladaAnimation(colladaFile);
            m.UseAnimation(anim);
            return m;
        }

        /**
         * Constructs the joint-hierarchy skeleton from the data extracted from the
         * collada file.
         * 
         * @param data
         *            - the joints data from the collada file for the head joint.
         * @return The created joint, with all its descendants added.
         */
        private static Joint CreateJoints(JointData data)
        {
            Joint joint = new Joint(data.Index, data.NameId, data.BindLocalTransform);
            foreach (JointData child in data.Children)
            {
                joint.AddChild(CreateJoints(child));
            }
            return joint;
        }
    }
}
