using BlobRPG.MainComponents;
using BlobRPG.Shaders;
using BlobRPG.Shaders.Filters;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Render.PostProcessing.Filters
{
    public class VerticalBlurFilter : PostProcessFilter
    {
        private readonly VerticalBlurShader Shader;

        public VerticalBlurFilter(Window window, float multiplier = 1) : base(window, multiplier)
        {
            Shader = new VerticalBlurShader();
        }
        public VerticalBlurFilter(int width, int height) : base(width, height)
        {
            Shader = new VerticalBlurShader();
        }

        public override void Render(int texture)
        {
            Shader.Start();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture);
            Renderer.Render();
            Shader.Stop();
        }
        public override void UpdateVariables()
        {
            Shader.Start();
            Shader.LoadHeight(Renderer.Height);
            Shader.Stop();
        }
        public override void CleanUp()
        {
            Shader.CleanUp();
            Renderer.CleanUp();
            Renderer.DestroyFBO();
        }
    }
}
