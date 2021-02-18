using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Shaders
{
    public class EntityShader : ShaderCore
    {
        public EntityShader() : base("entity")
        {
            
        }

        protected override void BindAttributes()
        {
            BindAttribute(0, "position");
        }
    }
}
