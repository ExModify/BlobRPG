using BlobRPG.Entities;
using BlobRPG.MainComponents;
using BlobRPG.Models;
using BlobRPG.Shaders;
using BlobRPG.Tools;
using GlmSharp;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Render
{
    public class WaterRenderer
    {
        readonly WaterShader Shader;
        readonly RawModel BaseQuad;

        public WaterRenderer(WaterShader shader, ref mat4 projectionMatrix)
        {
            Shader = shader;

            shader.Start();
            shader.LoadProjectionMatrix(ref projectionMatrix);
            shader.Stop();

            BaseQuad = Loader.LoadToVao(new float[] { -1, -1, -1, 1, 1, -1, 1, -1, -1, 1, 1, 1 });
        }

        public void Render(WaterTile tile, Camera camera)
        {
            Prepare(camera);

            mat4 modelMatrix = MatrixMaths.CreateTransformationMatrix(new vec3(tile.X, tile.Height, tile.Z), 0, 0, 0, WaterTile.TILE_SIZE);
            Shader.LoadTransformationMatrix(modelMatrix);
            GL.DrawArrays(PrimitiveType.Triangles, 0, BaseQuad.VertexCount);

            Finish();

        }
        private void Prepare(Camera camera)
        {
            Shader.Start();
            Shader.LoadViewMatrix(camera);
            GL.BindVertexArray(BaseQuad.VaoId);
            GL.EnableVertexAttribArray(0);
        }
        private void Finish()
        {
            GL.DisableVertexAttribArray(0);
            GL.BindVertexArray(0);
            Shader.Stop();
        }
    }
}
