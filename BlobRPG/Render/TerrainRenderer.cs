using BlobRPG.Entities;
using BlobRPG.Models;
using BlobRPG.Shaders;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace BlobRPG.Render
{
    public class TerrainRenderer
    {
        private TerrainShader Shader;

        public TerrainRenderer(TerrainShader shader, ref mat4 projectionMatrix)
        {
            Shader = shader;
            shader.Start();
            shader.LoadProjectionMatrix(projectionMatrix);
            shader.Stop();
        }

        public void Render(List<Terrain> terrains, Camera camera, Light light)
        {
            Prepare(camera, light);
            foreach (Terrain terrain in terrains)
            {
                PrepareTerrain(terrain);
                LoadModelMatrix(terrain);

                GL.DrawElements(PrimitiveType.Triangles, terrain.Model.VertexCount, DrawElementsType.UnsignedInt, 0);

                FinishTexturedModel();
            }
        }
        private void Prepare(Camera camera, Light light)
        {
            Shader.Start();
            Shader.LoadViewMatrix(camera);
            Shader.LoadLight(light);
        }

        private void PrepareTerrain(Terrain terrain)
        {
            GL.BindVertexArray(terrain.Model.VaoId);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, terrain.Texture.Id);

            Shader.LoadShineVariables(terrain.Texture.Reflectivity, terrain.Texture.ShineDamper);
        }
        private void LoadModelMatrix(Terrain terrain)
        {
            Shader.LoadTransformationMatrix(terrain.TransformationMatrix);
        }
        private void FinishTexturedModel()
        {
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);

            GL.BindVertexArray(0);
        }
    }
}
