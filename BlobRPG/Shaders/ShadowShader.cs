using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Shaders
{
    class ShadowShader : ShaderCore
    {
        private int MVPMatrixLocation;

        public ShadowShader() : base("shadow")
        {
        }

        protected override void GetAllUniformLocations()
        {
            MVPMatrixLocation = GetUniformLocation("mvpMatrix");
        }
        protected override void BindAttributes()
        {
            BindAttribute(0, "in_position");
            BindAttribute(1, "in_textureCoords");
        }
        public void LoadModelViewProjectionMatrix(mat4 matrix)
        {
            LoadMatrix(MVPMatrixLocation, matrix);
        }
    }
}
