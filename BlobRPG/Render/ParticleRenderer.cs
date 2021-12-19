using BlobRPG.Entities;
using BlobRPG.MainComponents;
using BlobRPG.Models;
using BlobRPG.Shaders;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Render
{
    public class ParticleRenderer
    {
        private readonly ParticleShader Shader;
        private RawModel Model;

        public ParticleRenderer(ParticleShader shader)
        {
            Shader = shader;

            float[] positions = new float[] { -0.5f, 0.5f, -0.5f, -0.5f, 0.5f, 0.5f, 0.5f, -0.5f };
            Model = Loader.LoadToVao(positions);
        }

        public void Render(List<Particle> particles)
        {
            foreach (Particle particle in particles)
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                //GL.BindTexture(TextureTarget.Texture2D, texture.TextureId);

                //Shader.LoadTransformationMatrix(texture.TransformationMatrix);


                GL.DrawArrays(PrimitiveType.TriangleStrip, 0, Model.VertexCount);
            }

        }
        private void PrepareRender()
        {
            Shader.Start();

            GL.BindVertexArray(Model.VaoId);
            GL.EnableVertexAttribArray(0);

            Renderer.EnableAlphaBlend();
            GL.Disable(EnableCap.DepthTest);

        }
        private void FinishRender()
        {
            Renderer.DisableAlphaBlend();
            GL.Enable(EnableCap.DepthTest);

            GL.DisableVertexAttribArray(0);
            GL.BindVertexArray(0);

            Shader.Stop();
        }
    }
}
