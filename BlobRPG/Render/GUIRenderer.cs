using OpenTK.Graphics.OpenGL;
using BlobRPG.MainComponents;
using BlobRPG.Models;
using BlobRPG.Textures;
using System;
using System.Collections.Generic;
using System.Text;
using BlobRPG.Shaders;

namespace BlobRPG.Render
{
    public class GUIRenderer
    {
        private readonly GUIShader Shader;
        private RawModel Model;

        public GUIRenderer(GUIShader shader)
        {
            Shader = shader;

            float[] positions = new float[] { -1, 1, -1, -1, 1, 1, 1, -1 };
            Model = Loader.LoadToVao(positions);
        }

        public void Render(List<GUITexture> guis)
        {
            Shader.Start();

            GL.BindVertexArray(Model.VaoId);
            GL.EnableVertexAttribArray(0);

            Renderer.EnableAlphaBlend();
            GL.Disable(EnableCap.DepthTest);

            foreach (GUITexture texture in guis)
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, texture.TextureId);

                Shader.LoadTransformationMatrix(texture.TransformationMatrix);


                GL.DrawArrays(PrimitiveType.TriangleStrip, 0, Model.VertexCount);
            }

            Renderer.DisableAlphaBlend();
            GL.Enable(EnableCap.DepthTest);

            GL.DisableVertexAttribArray(0);
            GL.BindVertexArray(0);

            Shader.Stop();
        }
    }
}
