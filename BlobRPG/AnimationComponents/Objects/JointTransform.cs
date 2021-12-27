using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.AnimationComponents.Objects
{
    public class JointTransform
    {
        public vec3 Position { get; private set; }
        public Quaternion Rotation { get; private set; }

        public mat4 LocalTransform
        {
            get
            {
                mat4 mat = mat4.Identity;
                mat *= mat4.Translate(Position);
                return mat * Rotation.RotationMatrix;
            }
        }

        public JointTransform(vec3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }
        public static JointTransform Interpolate(JointTransform frameA, JointTransform frameB, float progression)
        {
            vec3 pos = Interpolate(frameA.Position, frameB.Position, progression);
            Quaternion rot = Quaternion.Interpolate(frameA.Rotation, frameB.Rotation, progression);
            return new JointTransform(pos, rot);
        }
        private static vec3 Interpolate(vec3 start, vec3 end, float progression)
        {
            float x = start.x + (end.x - start.x) * progression;
            float y = start.y + (end.y - start.y) * progression;
            float z = start.z + (end.z - start.z) * progression;
            return new vec3(x, y, z);
        }
    }
}
