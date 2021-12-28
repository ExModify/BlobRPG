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
        public mat4 OriginalMatrix;

        public mat4 LocalTransform
        {
            get
            {
                return OriginalMatrix;
            }
        }

        public JointTransform(mat4 original)
        {
            OriginalMatrix = original;
        }
        public static JointTransform Interpolate(JointTransform frameA, JointTransform frameB, float progression)
        {
            mat4 interpolated = Interpolate(ref frameA.OriginalMatrix, ref frameB.OriginalMatrix, progression);
            return new JointTransform(interpolated);
        }

        private static mat4 Interpolate(ref mat4 prevFrame, ref mat4 nextFrame, float interpolation)
        {
            quat firstQuat = prevFrame.ToQuaternion;
            quat secondQuat = nextFrame.ToQuaternion;
            quat finalQuat = quat.SLerp(firstQuat, secondQuat, interpolation);
            mat4 rotationMatrix = finalQuat.ToMat4;

            vec3 scale = new vec3(prevFrame.m00, prevFrame.m11, prevFrame.m22);
            vec3 newScale = new vec3(nextFrame.m00, nextFrame.m11, nextFrame.m22);

            vec3 finalTrans = (float)(1.0 - interpolation) * scale + newScale * interpolation;

            rotationMatrix.m00 = finalTrans.x;
            rotationMatrix.m11 = finalTrans.z;
            rotationMatrix.m22 = finalTrans.y;

            return rotationMatrix;
        }
    }
}
