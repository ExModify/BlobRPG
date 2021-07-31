﻿using BlobRPG.Entities;
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
    public class NormalRenderer
    {
        readonly NormalShader Shader;

        public NormalRenderer(NormalShader shader, ref mat4 projectionMatrix)
        {
            Shader = shader;

            shader.Start();
            shader.LoadProjectionMatrix(projectionMatrix);
            shader.ConnectTextureUnits();
            shader.Stop();
        }

        public void Render(Dictionary<TexturedModel, List<Entity>> entities, vec4 clipPlane, Camera camera, List<Light> lights, Fog fog)
        {
            Prepare(clipPlane, camera, lights, fog);
            foreach (TexturedModel model in entities.Keys)
            {
                PrepareTexturedModel(model);

                foreach (Entity entity in entities[model])
                {
                    PrepareInstance(entity);
                    GL.DrawElements(PrimitiveType.Triangles, model.Model.VertexCount, DrawElementsType.UnsignedInt, 0);
                }

                FinishTexturedModel();
            }

            Shader.Stop();
        }
        private void Prepare(vec4 clipPlane, Camera camera, List<Light> lights, Fog fog)
        {
            Shader.Start();
            Shader.LoadClipPlane(clipPlane);
            Shader.LoadViewMatrix(camera);
            Shader.LoadLights(lights, camera);
            Shader.LoadFog(fog);
        }

        private void PrepareTexturedModel(TexturedModel model)
        {
            GL.BindVertexArray(model.Model.VaoId);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);

            if (model.Texture.HasTransparency) Renderer.DisableCulling();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, model.Texture.Id);

            Shader.LoadShineVariables(model.Texture.Reflectivity, model.Texture.ShineDamper);
            Shader.LoadFakeLighting(model.Texture.UseFakeLighting);
            Shader.LoadNumberOfRows(model.Texture.NumberOfRows);
        }
        private void PrepareInstance(Entity entity)
        {
            mat4 transformationMatrix = MatrixMaths.CreateTransformationMatrix(entity.Position, entity.RotationX, entity.RotationY, entity.RotationZ, entity.Scale);
            Shader.LoadTransformationMatrix(transformationMatrix);
            Shader.LoadTextureOffset(entity.CalculateTextureOffset());
        }
        private void FinishTexturedModel()
        {
            Renderer.EnableCulling();
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);

            GL.BindVertexArray(0);
        }
    }
}
