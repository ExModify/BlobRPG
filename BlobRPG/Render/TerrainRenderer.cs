﻿using BlobRPG.Entities;
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
            shader.ConnectTextureUnits();
            shader.LoadShadowVariables(Settings.ShadowDistance, Settings.ShadowMapSize, Settings.PCFCount);
            shader.LoadProjectionMatrix(projectionMatrix);
            shader.Stop();
        }

        public void Render(List<Terrain> terrains, Camera camera, List<Light> lights, Fog fog, vec4 clipPlane, ref mat4 toShadowSpace)
        {
            Prepare(camera, lights, fog, clipPlane, ref toShadowSpace);
            for (int i = 0; i < terrains.Count; i++)
            {
                PrepareTerrain(terrains[i]);

                GL.DrawElements(PrimitiveType.Triangles, terrains[i].Model.VertexCount, DrawElementsType.UnsignedInt, 0);

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

        private void PrepareTerrain(Terrain terrain)
        {
            GL.BindVertexArray(terrain.Model.VaoId);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, terrain.TexturePack.BackgroundTexture.TextureId);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, terrain.TexturePack.RTexture.TextureId);

            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, terrain.TexturePack.GTexture.TextureId);

            GL.ActiveTexture(TextureUnit.Texture3);
            GL.BindTexture(TextureTarget.Texture2D, terrain.TexturePack.BTexture.TextureId);

            GL.ActiveTexture(TextureUnit.Texture4);
            GL.BindTexture(TextureTarget.Texture2D, terrain.BlendMap.TextureId);

            Shader.LoadShineVariables(0, 1);
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
