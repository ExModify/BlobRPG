using GlmSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Shaders
{
    public class ParticleShader : ShaderCore
    {
        private int ProjectionMatrixLocation;
        private int NumberOfRowsLocation;

        public ParticleShader() : base("particle")
        {
        }

        protected override void GetAllUniformLocations()
        {
            ProjectionMatrixLocation = GetUniformLocation("projectionMatrix");
            NumberOfRowsLocation = GetUniformLocation("numberOfRows");
        }
        protected override void BindAttributes()
        {
            BindAttribute(0, "position");
            BindAttribute(1, "modelViewMatrix");
            BindAttribute(5, "textureOffsets");
            BindAttribute(6, "blendFactor");
        }
        public void LoadProjectionMatrix(ref mat4 matrix)
        {
            LoadMatrix(ProjectionMatrixLocation, matrix);
        }
        public void LoadNumberOfRows(float numberOfRows)
        {
            LoadFloat(NumberOfRowsLocation, numberOfRows);
        }
    }
}
