using BlobRPG.Entities;
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
        public static mat4 CreateViewMatrix(Camera camera)
        {
            mat4 matrix = mat4.Identity;

            matrix *= mat4.RotateX(MathHelper.DegreesToRadians(camera.Pitch));
            matrix *= mat4.RotateY(MathHelper.DegreesToRadians(camera.Yaw));

            vec3 negCamPos = -camera.Position;

            matrix *= mat4.Translate(negCamPos);

            return matrix;
        }

        public static mat4 CreateTransformationMatrix(vec2 translation, vec2 scale)
        {
            mat4 matrix = mat4.Identity;

            matrix *= mat4.Translate(translation.x, translation.y, 1);
            matrix *= mat4.Scale(scale.x, scale.y, 1);

            return matrix;
        }
    }
}
