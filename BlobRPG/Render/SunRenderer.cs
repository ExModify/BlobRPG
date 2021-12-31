using BlobRPG.Entities;
using BlobRPG.MainComponents;
using BlobRPG.Models;
using BlobRPG.Shaders;
using BlobRPG.Textures;
using GlmSharp;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Render
{
    public class SunRenderer
    {
        RawModel Quad { get; set; }
        SunShader Shader { get; set; }

        public SunRenderer(SunShader shader, ref mat4 projectionMatrix)
        {
            Quad = Loader.LoadToVao(new float[] { -0.5f, 0.5f, -0.5f, -0.5f, 0.5f, 0.5f, 0.5f, -0.5f });

            Shader = shader;

            Shader.Start();
            Shader.LoadProjectionMatrix(ref projectionMatrix);
            Shader.Stop();
        }

        public void Render(Camera camera, vec4 clipPlane)
        {
            Shader.Start();
            Shader.LoadClipPlane(clipPlane);
            Shader.LoadViewMatrix(camera);

            GL.BindVertexArray(Quad.VaoId);
            GL.EnableVertexAttribArray(0);
            GL.DepthMask(false);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Settings.Sun.CurrentTexture.Id);

            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, Quad.VertexCount);

            GL.BindVertexArray(0);
            GL.EnableVertexAttribArray(1);
            GL.DepthMask(true);
            GL.Disable(EnableCap.Blend);
            Shader.Stop();
        }
    }
}
