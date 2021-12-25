using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Shaders.Filters
{
    public class VerticalBlurShader : ShaderCore
    {

        private int TargetHeightLocation;
        public VerticalBlurShader() : base("verticalBlur", "gaussianBlur")
        {

        }
        protected override void BindAttributes()
        {
            BindAttribute(0, "position");
        }

        protected override void GetAllUniformLocations()
        {
            TargetHeightLocation = GetUniformLocation("targetHeight");
        }

        public void LoadHeight(float height)
        {
            LoadFloat(TargetHeightLocation, height);
        }
    }
}
