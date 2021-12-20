using BlobRPG.Entities;
using BlobRPG.MainComponents;
using BlobRPG.Models;
using BlobRPG.Shaders;
using BlobRPG.Textures;
using BlobRPG.Tools;
using GlmSharp;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Render
{
    public class ParticleRenderer
    {
        private const int MaxInstances = 10000;
        private const int InstanceDataLength = 21;

        private RawModel Quad;

        public ParticleShader Shader;

        private int VBO;
        private int Pointer = 0;

        public ParticleRenderer(ParticleShader shader, ref mat4 projectionMatrix)
        {
            VBO = Loader.CreateEmptyVbo(InstanceDataLength * MaxInstances);

            Quad = Loader.LoadToVao(new float[] { -0.5f, 0.5f, -0.5f, -0.5f, 0.5f, 0.5f, 0.5f, -0.5f });

            Loader.AddInstancedAttribute(Quad.VaoId, VBO, 1, 4, InstanceDataLength, 0);
            Loader.AddInstancedAttribute(Quad.VaoId, VBO, 2, 4, InstanceDataLength, 4);
            Loader.AddInstancedAttribute(Quad.VaoId, VBO, 3, 4, InstanceDataLength, 8);
            Loader.AddInstancedAttribute(Quad.VaoId, VBO, 4, 4, InstanceDataLength, 12);
            Loader.AddInstancedAttribute(Quad.VaoId, VBO, 5, 4, InstanceDataLength, 16);
            Loader.AddInstancedAttribute(Quad.VaoId, VBO, 6, 1, InstanceDataLength, 20);

            Shader = shader;

            Shader.Start();
            Shader.LoadProjectionMatrix(ref projectionMatrix);
            Shader.Stop();
        }

        public void Render(Dictionary<ParticleTexture, List<Particle>> particlesBundle, Camera camera)
        {
            mat4 ViewMatrix = camera.ViewMatrix;
            Prepare();
            foreach (ParticleTexture texture in particlesBundle.Keys)
            {
                List<Particle> particles = particlesBundle[texture];
                BindTexture(texture);
                Pointer = 0;

                float[] vboData = new float[particles.Count * InstanceDataLength];

                foreach (Particle particle in particles)
                {
                    UpdateModelViewMatrix(particle.Position, particle.Rotation, particle.Scale, ViewMatrix, ref vboData);
                    UpdateTextureCoordsInfo(particle, ref vboData);
                }

                Loader.UpdateVBO(VBO, vboData);

                GL.DrawArraysInstanced(PrimitiveType.TriangleStrip, 0, Quad.VertexCount, particles.Count);
            }

            EndRendering();
        }

        private void BindTexture(ParticleTexture Texture)
        {
            if (Texture.Additive)
            {
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
            }
            else
            {
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            }
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Texture.TextureId);
            Shader.LoadNumberOfRows(Texture.NumberOfRows);
        }

        private void UpdateModelViewMatrix(vec3 position, float rotation, float Scale, mat4 viewMatrix, ref float[] vboData)
        {
            mat4 modelMatrix = mat4.Translate(position.x, position.y, position.z);
            modelMatrix.m00 = viewMatrix.m00;
            modelMatrix.m01 = viewMatrix.m10;
            modelMatrix.m02 = viewMatrix.m20;
            modelMatrix.m10 = viewMatrix.m01;
            modelMatrix.m11 = viewMatrix.m11;
            modelMatrix.m12 = viewMatrix.m21;
            modelMatrix.m20 = viewMatrix.m02;
            modelMatrix.m21 = viewMatrix.m12;
            modelMatrix.m22 = viewMatrix.m22;

            mat4 modelViewMatrix = viewMatrix * modelMatrix;

            modelViewMatrix *= mat4.RotateZ(MathHelper.DegreesToRadians(rotation));
            modelViewMatrix *= mat4.Scale(Scale);

            StoreMatrixData(modelViewMatrix, ref vboData);
        }
        private void UpdateTextureCoordsInfo(Particle particle, ref float[] vboData)
        {
            vboData[Pointer++] = particle.TextureOffset1.x;
            vboData[Pointer++] = particle.TextureOffset1.y;
            vboData[Pointer++] = particle.TextureOffset2.x;
            vboData[Pointer++] = particle.TextureOffset2.y;

            vboData[Pointer++] = particle.BlendFactor;
        }

        private void StoreMatrixData(mat4 matrix, ref float[] vboData)
        {
            vboData[Pointer++] = matrix.m00;
            vboData[Pointer++] = matrix.m01;
            vboData[Pointer++] = matrix.m02;
            vboData[Pointer++] = matrix.m03;

            vboData[Pointer++] = matrix.m10;
            vboData[Pointer++] = matrix.m11;
            vboData[Pointer++] = matrix.m12;
            vboData[Pointer++] = matrix.m13;

            vboData[Pointer++] = matrix.m20;
            vboData[Pointer++] = matrix.m21;
            vboData[Pointer++] = matrix.m22;
            vboData[Pointer++] = matrix.m23;

            vboData[Pointer++] = matrix.m30;
            vboData[Pointer++] = matrix.m31;
            vboData[Pointer++] = matrix.m32;
            vboData[Pointer++] = matrix.m33;
        }

        private void Prepare()
        {
            Shader.Start();
            GL.BindVertexArray(Quad.VaoId);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.EnableVertexAttribArray(3);
            GL.EnableVertexAttribArray(4);
            GL.EnableVertexAttribArray(5);
            GL.EnableVertexAttribArray(6);
            GL.Enable(EnableCap.Blend);
            GL.DepthMask(false);
        }

        private void EndRendering()
        {
            GL.DepthMask(true);
            GL.Disable(EnableCap.Blend);
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);
            GL.DisableVertexAttribArray(3);
            GL.DisableVertexAttribArray(4);
            GL.DisableVertexAttribArray(5);
            GL.DisableVertexAttribArray(6);
            GL.BindVertexArray(0);
            Shader.Stop();
        }
    }
}
