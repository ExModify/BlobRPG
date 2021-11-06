using BlobRPG.Entities;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Shaders
{
    public class WaterShader : ShaderCore
    {
        private int TransformationMatrixLocation;
        private int ViewMatrixLocation;
        private int ProjectionMatrixLocation;

        public WaterShader() : base("water")
        {
        }
        protected override void BindAttributes()
        {
            BindAttribute(0, "position");
        }

        protected override void GetAllUniformLocations()
        {
            TransformationMatrixLocation = GetUniformLocation("transformationMatrix");
            ViewMatrixLocation = GetUniformLocation("viewMatrix");
            ProjectionMatrixLocation = GetUniformLocation("projectionMatrix");
        }
        public void LoadTransformationMatrix(mat4 matrix)
        {
            LoadMatrix(TransformationMatrixLocation, matrix);
        }
        public void LoadViewMatrix(Camera camera)
        {
            LoadMatrix(ViewMatrixLocation, camera.ViewMatrix);
        }
        public void LoadProjectionMatrix(ref mat4 matrix)
        {
            LoadMatrix(ProjectionMatrixLocation, matrix);
        }
    }
}
