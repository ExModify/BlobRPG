using BlobRPG.Entities;
using BlobRPG.MainComponents;
using BlobRPG.Models;
using BlobRPG.Shaders;
using BlobRPG.Textures;
using GlmSharp;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlobRPG.Render
{
    public class Renderer
    {
        private const float FOV = 70;
        private const float NEAR = 0.1f;
        private const float FAR = 1000f;

        mat4 ProjectionMatrix;

        readonly EntityRenderer EntityRenderer;
        readonly EntityShader EntityShader;
        readonly GUIRenderer GUIRenderer;

        readonly TerrainRenderer TerrainRenderer;
        readonly TerrainShader TerrainShader;
        readonly GUIShader GUIShader;

        readonly Dictionary<TexturedModel, List<Entity>> Entities;
        readonly List<Terrain> Terrains;
        readonly List<GUITexture> GUIs;

        readonly Window Window;

        public static void EnableCulling()
        {
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
        }
        public static void EnableAlphaBlend()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }
        public static void DisableCulling()
        {
            GL.Disable(EnableCap.CullFace);
        }
        public static void DisableAlphaBlend()
        {
            GL.Disable(EnableCap.Blend);
        }

        public Renderer(Window window)
        {
            Window = window;

            Entities = new Dictionary<TexturedModel, List<Entity>>();
            Terrains = new List<Terrain>();
            GUIs = new List<GUITexture>();

            EnableCulling();

            window.Resize += (s) =>
            {
                CreateProjectionMatrix(FOV, s.Width, s.Height, NEAR, FAR);
                UpdateProjectionMatrix();
            };


            CreateProjectionMatrix(FOV, window.ClientSize.X, window.ClientSize.Y, NEAR, FAR);

            EntityShader = new EntityShader();
            EntityRenderer = new EntityRenderer(EntityShader, ref ProjectionMatrix);

            TerrainShader = new TerrainShader();
            TerrainRenderer = new TerrainRenderer(TerrainShader, ref ProjectionMatrix);

            GUIShader = new GUIShader();
            GUIRenderer = new GUIRenderer(GUIShader);
        }
        
        public void Render(Camera camera, Light light, Fog fog)
        {
            Prepare();

            EntityRenderer.Render(Entities, camera, light, fog);

            TerrainRenderer.Render(Terrains, camera, light, fog);

            GUIRenderer.Render(GUIs);

            Terrains.Clear();
            Entities.Clear();
        }
        public void CleanUp()
        {
            EntityShader.CleanUp();
            TerrainShader.CleanUp();
            GUIShader.CleanUp();
        }
        
        public void AddGUI(GUITexture texture)
        {
            GUIs.Add(texture);
        }

        public void ProcessTerrain(Terrain terrain)
        {
            Terrains.Add(terrain);
        }
        public void ProcessObject(Entity entity)
        {
            if (Entities.Keys.Contains(entity.Model))
            {
                Entities[entity.Model].Add(entity);
            }
            else
            {
                Entities.Add(entity.Model, new List<Entity>() { entity });
            }
        }

        private void Prepare()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Window.SkyColor.x, Window.SkyColor.y, Window.SkyColor.z, 1);
        }

        private void CreateProjectionMatrix(float fov, int width, int height, float near, float far)
        {
            ProjectionMatrix = mat4.PerspectiveFov(fov, width, height, near, far);
        }

        private void UpdateProjectionMatrix()
        {
            EntityShader.Start();
            EntityShader.LoadProjectionMatrix(ProjectionMatrix);
            EntityShader.Stop();

            TerrainShader.Start();
            TerrainShader.LoadProjectionMatrix(ProjectionMatrix);
            TerrainShader.Stop();
        }
    }
}
