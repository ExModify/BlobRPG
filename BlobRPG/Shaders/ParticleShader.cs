using GlmSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Shaders
{
    public class ParticleShader : ShaderCore
    {
        private int ProjectionMatrixLocation;
        private int ModelViewMatrixLocation;

        public ParticleShader() : base("particle")
        {
        }

        protected override void GetAllUniformLocations()
        {
            ProjectionMatrixLocation = GetUniformLocation("projectionMatrix");
            ModelViewMatrixLocation = GetUniformLocation("modelViewMatrix");
        }
        protected override void BindAttributes()
        {
            BindAttribute(0, "position");
        }
        public void LoadProjectionMatrix(ref mat4 matrix)
        {
            LoadMatrix(ProjectionMatrixLocation, matrix);
        }
        public void LoadModelViewMatrix(mat4 matrix)
        {
            LoadMatrix(ModelViewMatrixLocation, matrix);
        }
    }
}
