using GlmSharp;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Tools
{
    public static class MatrixMaths
    {
        public static mat4 CreateTransformationMatrix(vec3 translation, float rx, float ry, float rz, float scale)
        {
            mat4 matrix = mat4.Identity;

            matrix *= mat4.Translate(translation);
            matrix *= mat4.RotateX(MathHelper.DegreesToRadians(rx));
            matrix *= mat4.RotateY(MathHelper.DegreesToRadians(ry));
            matrix *= mat4.RotateZ(MathHelper.DegreesToRadians(rz));
            matrix *= mat4.Scale(scale);

            return matrix;
        }
    }
}
