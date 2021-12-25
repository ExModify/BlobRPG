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
    public class ContrastFilter : PostProcessFilter
    {
        private readonly ContrastShader Shader;

        public ContrastFilter(Window window) : base(window)
        {
            Shader = new ContrastShader();
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
            Shader.LoadContrast(Settings.Contrast);
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
