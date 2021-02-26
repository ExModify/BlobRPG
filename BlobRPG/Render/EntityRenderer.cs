using BlobRPG.Entities;
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
    public class EntityRenderer
    {
        readonly EntityShader Shader;

        public EntityRenderer(EntityShader shader, ref mat4 projectionMatrix)
        {
            Shader = shader;

            shader.Start();
            shader.LoadProjectionMatrix(projectionMatrix);
            shader.Stop();
        }

        public void Render(Dictionary<TexturedModel, List<Entity>> entities, Camera camera, Light light, Fog fog)
        {
            Prepare(camera, light, fog);
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
        private void Prepare(Camera camera, Light light, Fog fog)
        {
            Shader.Start();
            Shader.LoadViewMatrix(camera);
            Shader.LoadLight(light);
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
