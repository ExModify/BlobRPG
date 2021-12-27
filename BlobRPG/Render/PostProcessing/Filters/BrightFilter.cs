using BlobRPG.MainComponents;
using BlobRPG.Shaders.Filters;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Render.PostProcessing.Filters
{
    public class BrightFilter : PostProcessFilter
    {
        private readonly BrightShader Shader;

        public BrightFilter(Window window, float multiplier = 1) : base(window, multiplier)
        {
            Shader = new BrightShader();
        }

        public override void Render(int texture)
        {
            Shader.Start();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture);
            Renderer.Render();
            Shader.Stop();
        }
        public override void CleanUp()
        {
            Shader.CleanUp();
            Renderer.CleanUp();
            Renderer.DestroyFBO();
        }
        public override void UpdateVariables()
        {
            
        }
    }
}
