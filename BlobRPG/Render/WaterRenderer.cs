using BlobRPG.Entities;
using BlobRPG.MainComponents;
using BlobRPG.Models;
using BlobRPG.Render.Water;
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
        readonly int DUDVTexture;
        readonly int NormalTexture;
        readonly Window Window;
        WaterFrameBuffers FBOs;

        private float MoveFactor = 0;



        public WaterRenderer(WaterShader shader, ref mat4 projectionMatrix, float nearPlane, float farPlane, Window window, WaterFrameBuffers fbos, int dudvTexture, int normalTexture)
        {
            Shader = shader;

            shader.Start();
            shader.ConnectTextureUnits();
            shader.LoadPlaneVariables(nearPlane, farPlane);
            shader.LoadWaveStrength(0.04f);
            shader.LoadTiling(4);
            shader.LoadShineVariables(20, 0.5f);
            shader.LoadProjectionMatrix(ref projectionMatrix);
            shader.Stop();

            BaseQuad = Loader.LoadToVao(new float[] { -1, -1, -1, 1, 1, -1, 1, -1, -1, 1, 1, 1 });

            Window = window;
            FBOs = fbos;
            DUDVTexture = dudvTexture;
            NormalTexture = normalTexture;
        }

        public void Render(WaterTile tile, Camera camera, Light sun)
        {
            Prepare(camera, sun);

            mat4 modelMatrix = MatrixMaths.CreateTransformationMatrix(new vec3(tile.X, tile.Height, tile.Z), 0, 0, 0, WaterTile.TILE_SIZE);
            Shader.LoadTransformationMatrix(modelMatrix);
            GL.DrawArrays(PrimitiveType.Triangles, 0, BaseQuad.VertexCount);

            Finish();

        }
        private void Prepare(Camera camera, Light sun)
        {
            Shader.Start();
            Shader.LoadViewMatrix(camera);
            MoveFactor += (float)(Program.WAVE_SPEED * Window.DeltaTime);
            MoveFactor %= 1;
            Shader.LoadMoveFactor(MoveFactor);
            Shader.LoadSun(sun);

            GL.BindVertexArray(BaseQuad.VaoId);
            GL.EnableVertexAttribArray(0);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, FBOs.ReflectionTexture);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, FBOs.RefractionTexture);

            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, DUDVTexture);

            GL.ActiveTexture(TextureUnit.Texture3);
            GL.BindTexture(TextureTarget.Texture2D, NormalTexture);

            GL.ActiveTexture(TextureUnit.Texture4);
            GL.BindTexture(TextureTarget.Texture2D, FBOs.RefractionDepthTexture);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }
        private void Finish()
        {
            GL.Disable(EnableCap.Blend);
            GL.DisableVertexAttribArray(0);
            GL.BindVertexArray(0);
            Shader.Stop();
        }
    }
}
