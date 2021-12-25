using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Shaders.Filters
{
    public class ContrastShader : ShaderCore
    {
        private int ContrastLocation;


        public ContrastShader() : base("contrast")
        {
        }

        protected override void GetAllUniformLocations()
        {
            ContrastLocation = GetUniformLocation("contrast");
        }
        protected override void BindAttributes()
        {
            BindAttribute(0, "position");
        }

        public void LoadContrast(float contrast)
        {
            LoadFloat(ContrastLocation, Math.Clamp(contrast, 0, 1));
        }
    }
}
