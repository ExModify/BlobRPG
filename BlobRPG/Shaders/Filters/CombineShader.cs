using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Shaders.Filters
{
    class CombineShader : ShaderCore
    {
        private int ColorTextureLocation;
        private int HighlightTextureLocation;

        public CombineShader() : base("simple", "combine")
        {

        }
        protected override void GetAllUniformLocations()
        {
            ColorTextureLocation = GetUniformLocation("colorTexture");
            HighlightTextureLocation = GetUniformLocation("highlightTexture");
        }
        protected override void BindAttributes()
        {
            BindAttribute(0, "position");
        }
        public void ConnectTextureUnits()
        {
            LoadInt(ColorTextureLocation, 0);
            LoadInt(HighlightTextureLocation, 1);
        }

    }
}
