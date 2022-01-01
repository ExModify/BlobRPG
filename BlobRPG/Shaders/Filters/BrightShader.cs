using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Shaders.Filters
{
    public class BrightShader : ShaderCore
    {
        public BrightShader() : base("simple", "bright")
        {

        }
        protected override void GetAllUniformLocations()
        {
        }
        protected override void BindAttributes()
        {
            BindAttribute(0, "position");
        }
    }
}
