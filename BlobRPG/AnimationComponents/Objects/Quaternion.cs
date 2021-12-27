using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.AnimationComponents.Objects
{
    public class Quaternion
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }
        public float W { get; private set; }

		public mat4 RotationMatrix
        {
            get
			{
				mat4 matrix = mat4.Identity;
				float xy = X * Y;
				float xz = X * Z;
				float xw = X * W;
				float yz = Y * Z;
				float yw = Y * W;
				float zw = Z * W;
				float xSquared = X * X;
				float ySquared = Y * Y;
				float zSquared = Z * Z;
				matrix.m00 = 1 - 2 * (ySquared + zSquared);
				matrix.m01 = 2 * (xy - zw);
				matrix.m02 = 2 * (xz + yw);
				matrix.m03 = 0;
				matrix.m10 = 2 * (xy + zw);
				matrix.m11 = 1 - 2 * (xSquared + zSquared);
				matrix.m12 = 2 * (yz - xw);
				matrix.m13 = 0;
				matrix.m20 = 2 * (xz - yw);
				matrix.m21 = 2 * (yz + xw);
				matrix.m22 = 1 - 2 * (xSquared + ySquared);
				matrix.m23 = 0;
				matrix.m30 = 0;
				matrix.m31 = 0;
				matrix.m32 = 0;
				matrix.m33 = 1;
				return matrix;
			}
        }

        public Quaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
            Normalize();
        }
        public void Normalize()
        {
            float mag = (float)Math.Sqrt(W * W + X * X + Y * Y + Z * Z);
            W /= mag;
            X /= mag;
            Y /= mag;
            Z /= mag;
        }
		public static Quaternion FromMatrix(mat4 matrix)
        {
			float w, x, y, z;
			float diagonal = matrix.m00 + matrix.m11 + matrix.m22;
			if (diagonal > 0)
			{
				float w4 = (float)(Math.Sqrt(diagonal + 1f) * 2f);
				w = w4 / 4f;
				x = (matrix.m21 - matrix.m12) / w4;
				y = (matrix.m02 - matrix.m20) / w4;
				z = (matrix.m10 - matrix.m01) / w4;
			}
			else if ((matrix.m00 > matrix.m11) && (matrix.m00 > matrix.m22))
			{
				float x4 = (float)(Math.Sqrt(1f + matrix.m00 - matrix.m11 - matrix.m22) * 2f);
				w = (matrix.m21 - matrix.m12) / x4;
				x = x4 / 4f;
				y = (matrix.m01 + matrix.m10) / x4;
				z = (matrix.m02 + matrix.m20) / x4;
			}
			else if (matrix.m11 > matrix.m22)
			{
				float y4 = (float)(Math.Sqrt(1f + matrix.m11 - matrix.m00 - matrix.m22) * 2f);
				w = (matrix.m02 - matrix.m20) / y4;
				x = (matrix.m01 + matrix.m10) / y4;
				y = y4 / 4f;
				z = (matrix.m12 + matrix.m21) / y4;
			}
			else
			{
				float z4 = (float)(Math.Sqrt(1f + matrix.m22 - matrix.m00 - matrix.m11) * 2f);
				w = (matrix.m10 - matrix.m01) / z4;
				x = (matrix.m02 + matrix.m20) / z4;
				y = (matrix.m12 + matrix.m21) / z4;
				z = z4 / 4f;
			}
			return new Quaternion(x, y, z, w);
		}
		public static Quaternion Interpolate(Quaternion a, Quaternion b, float blend)
		{
			Quaternion result = new Quaternion(0, 0, 0, 1);
			float dot = a.W * b.W + a.X * b.X + a.Y * b.Y + a.Z * b.Z;
			float blendI = 1f - blend;
			if (dot < 0)
			{
				result.W = blendI * a.W + blend * -b.W;
				result.X = blendI * a.X + blend * -b.X;
				result.Y = blendI * a.Y + blend * -b.Y;
				result.Z = blendI * a.Z + blend * -b.Z;
			}
			else
			{
				result.W = blendI * a.W + blend * b.W;
				result.X = blendI * a.X + blend * b.X;
				result.Y = blendI * a.Y + blend * b.Y;
				result.Z = blendI * a.Z + blend * b.Z;
			}
			result.Normalize();
			return result;
		}
	}
}
