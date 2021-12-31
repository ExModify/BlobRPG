using BlobRPG.Entities;
using GlmSharp;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Shaders
{
    public class SunShader : ShaderCore
    {
        private int ProjectionMatrixLocation;
        private int ViewMatrixLocation;

        private int ClipPlaneLocation;

        public SunShader() : base("sun")
        {
        }

        protected override void BindAttributes()
        {
            BindAttribute(0, "position");
        }



        protected override void GetAllUniformLocations()
        {
            ProjectionMatrixLocation = GetUniformLocation("projectionMatrix");
            ViewMatrixLocation = GetUniformLocation("viewMatrix");

            ClipPlaneLocation = GetUniformLocation("clipPlane");
        }

        public void LoadClipPlane(vec4 clipPlane)
        {
            LoadVector(ClipPlaneLocation, clipPlane);
        }

        public void LoadProjectionMatrix(ref mat4 matrix)
        {
            LoadMatrix(ProjectionMatrixLocation, matrix);
        }
        public void LoadViewMatrix(Camera camera)
        {
            mat4 viewMatrix = camera.ViewMatrix;
            mat4 modelMatrix = mat4.Translate(Settings.Sun.Position.x, Settings.Sun.Position.y, Settings.Sun.Position.z);
            modelMatrix.m00 = viewMatrix.m00;
            modelMatrix.m01 = viewMatrix.m10;
            modelMatrix.m02 = viewMatrix.m20;
            modelMatrix.m10 = viewMatrix.m01;
            modelMatrix.m11 = viewMatrix.m11;
            modelMatrix.m12 = viewMatrix.m21;
            modelMatrix.m20 = viewMatrix.m02;
            modelMatrix.m21 = viewMatrix.m12;
            modelMatrix.m22 = viewMatrix.m22;

            mat4 modelViewMatrix = viewMatrix * modelMatrix;

            modelViewMatrix *= mat4.Scale(Settings.SunSize);

            LoadMatrix(ViewMatrixLocation, modelViewMatrix);
        }
    }
}
