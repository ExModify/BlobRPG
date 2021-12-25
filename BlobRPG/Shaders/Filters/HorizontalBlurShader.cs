using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Shaders.Filters
{
    public class HorizontalBlurShader : ShaderCore
    {
        private int TargetWidthLocation;

        public HorizontalBlurShader() : base("horizontalBlur", "gaussianBlur")
        {

        }
        protected override void BindAttributes()
        {
            BindAttribute(0, "position");
        }

        protected override void GetAllUniformLocations()
        {
            TargetWidthLocation = GetUniformLocation("targetWidth");
        }

        public void LoadWidth(float width)
        {
            LoadFloat(TargetWidthLocation, width);
        }
    }
}
