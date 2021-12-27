using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.AnimationComponents.Objects
{
    public class Joint
    {
        public int Index { get; private set; }
        public string Name { get; private set; }
        public List<Joint> Children { get; private set; }

        public mat4 AnimatedTransform { get; set; }

        public mat4 LocalBindTransform { get; private set; }
        public mat4 InverseBindTransform { get; private set; }


        public Joint(int index, string name, mat4 bindLocalTransform)
        {
            Index = index;
            Name = name;
            LocalBindTransform = bindLocalTransform;
            Children = new List<Joint>();
            AnimatedTransform = InverseBindTransform = mat4.Identity;
        }
        public void AddChild(Joint child)
        {
            Children.Add(child);
        }
        public void CalculateInverseBindTranform(ref mat4 parentBindTransform)
        {
            mat4 bindTransform = parentBindTransform * LocalBindTransform;
            InverseBindTransform = bindTransform.Inverse;
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].CalculateInverseBindTranform(ref bindTransform);
            }
        }
    }
}
