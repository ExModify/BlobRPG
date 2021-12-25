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
    public class HorizontalBlurFilter : PostProcessFilter
    {
        private readonly HorizontalBlurShader Shader;

        public HorizontalBlurFilter(Window window) : base(window)
        {
            Shader = new HorizontalBlurShader();
        }
        public HorizontalBlurFilter(int width, int height) : base(width, height)
        {
            Shader = new HorizontalBlurShader();
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
            Shader.LoadWidth(Renderer.Width);
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
