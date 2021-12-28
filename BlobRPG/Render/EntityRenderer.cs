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
            shader.ConnectTextureUnits();
            shader.LoadShadowVariables(Settings.ShadowDistance, Settings.ShadowMapSize, Settings.PCFCount);
            shader.LoadProjectionMatrix(projectionMatrix);
            shader.Stop();
        }

        public void Render(Dictionary<TexturedModel, List<Entity>> entities, Camera camera, List<Light> lights, Fog fog, vec4 clipPlane, ref mat4 toShadowSpace)
        {
            Prepare(camera, lights, fog, clipPlane, ref toShadowSpace);
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
        private void Prepare(Camera camera, List<Light> lights, Fog fog, vec4 clipPlane, ref mat4 toShadowSpace)
        {
            Shader.Start();
            Shader.LoadShadowMapSpace(ref toShadowSpace);
            Shader.LoadViewMatrix(camera);
            Shader.LoadLights(lights);
            Shader.LoadFog(fog);
            Shader.LoadClipPlane(clipPlane);
        }

        private void PrepareTexturedModel(TexturedModel model)
        {
            GL.BindVertexArray(model.Model.VaoId);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.EnableVertexAttribArray(3);
            GL.EnableVertexAttribArray(4);
            GL.EnableVertexAttribArray(5);

            if (model.Texture.HasTransparency) Renderer.DisableCulling();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, model.Texture.Id);

            if (model.Texture.HasSpecularMap)
            {
                GL.ActiveTexture(TextureUnit.Texture6);
                GL.BindTexture(TextureTarget.Texture2D, model.Texture.SpecularMap);
            }
            Shader.LoadSpecularMap(model.Texture.HasSpecularMap);
            Shader.LoadShineVariables(model.Texture.Reflectivity, model.Texture.ShineDamper);
            Shader.LoadFakeLighting(model.Texture.UseFakeLighting);
            Shader.LoadNumberOfRows(model.Texture.NumberOfRows);

            if (model.Animated)
            {
                // shader.loadjointtransforms
                // shader.loadanimate true
                Shader.LoadJointTransforms(model.AnimatedModel.GetJointTransforms());
            }
            else
            {
                Shader.LoadTransformCount(0);
                // shader.loadanimate false
            }
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
            GL.DisableVertexAttribArray(3);
            GL.DisableVertexAttribArray(4);
            GL.DisableVertexAttribArray(5);

            GL.BindVertexArray(0);
        }
    }
}
