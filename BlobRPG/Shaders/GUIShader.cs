using GlmSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Shaders
{
    public class GUIShader : ShaderCore
    {
        private int TransformationMatrixLocation;

        public GUIShader() : base("gui")
        {
        }

        protected override void GetAllUniformLocations()
        {
            TransformationMatrixLocation = GetUniformLocation("transformationMatrix");
        }
        protected override void BindAttributes()
        {
            BindAttribute(0, "position");
        }
        public void LoadTransformationMatrix(mat4 matrix)
        {
            LoadMatrix(TransformationMatrixLocation, matrix);
        }
    }
}
