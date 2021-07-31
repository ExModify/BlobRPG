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

        internal readonly EntityRenderer EntityRenderer;
        internal readonly NormalRenderer NormalRenderer;
        readonly TerrainRenderer TerrainRenderer;
        internal readonly GUIRenderer GUIRenderer;
        internal readonly SkyboxRenderer SkyboxRenderer;

        readonly EntityShader EntityShader;
        readonly NormalShader NormalShader;
        readonly TerrainShader TerrainShader;
        readonly GUIShader GUIShader;
        readonly SkyboxShader SkyboxShader;

        readonly Dictionary<TexturedModel, List<Entity>> Entities;
        readonly Dictionary<TexturedModel, List<Entity>> NormalEntities;
        readonly List<Terrain> Terrains;
        readonly List<GUITexture> GUIs;

        readonly Window Window;


        public mat4 ProjectionMatrix;


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
            NormalEntities = new Dictionary<TexturedModel, List<Entity>>();
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

            NormalShader = new NormalShader();
            NormalRenderer = new NormalRenderer(NormalShader, ref ProjectionMatrix);

            TerrainShader = new TerrainShader();
            TerrainRenderer = new TerrainRenderer(TerrainShader, ref ProjectionMatrix);

            GUIShader = new GUIShader();
            GUIRenderer = new GUIRenderer(GUIShader);

            SkyboxShader = new SkyboxShader(window);
            SkyboxRenderer = new SkyboxRenderer(SkyboxShader, window, ref ProjectionMatrix);
        }
        public void Update()
        {
            SkyboxRenderer.Update();
        }
        
        public void Render(Camera camera, List<Light> lights, Fog fog, vec4 clipPlane)
        {
            Prepare();

            EntityRenderer.Render(Entities, camera, lights, fog);

            NormalRenderer.Render(NormalEntities, clipPlane, camera, lights, fog);

            TerrainRenderer.Render(Terrains, camera, lights, fog);

            SkyboxRenderer.Render(camera, fog);

            GUIRenderer.Render(GUIs);

            Terrains.Clear();
            Entities.Clear();
            NormalEntities.Clear();
        }
        public void CleanUp()
        {
            EntityShader.CleanUp();
            NormalShader.CleanUp();
            TerrainShader.CleanUp();
            GUIShader.CleanUp();
            SkyboxShader.CleanUp();
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
        public void ProcessNormalObject(Entity entity)
        {
            if (NormalEntities.Keys.Contains(entity.Model))
            {
                NormalEntities[entity.Model].Add(entity);
            }
            else
            {
                NormalEntities.Add(entity.Model, new List<Entity>() { entity });
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

            NormalShader.Start();
            NormalShader.LoadProjectionMatrix(ProjectionMatrix);
            NormalShader.Stop();

            TerrainShader.Start();
            TerrainShader.LoadProjectionMatrix(ProjectionMatrix);
            TerrainShader.Stop();

            SkyboxShader.Start();
            SkyboxShader.LoadProjectionMatrix(ref ProjectionMatrix);
            SkyboxShader.Stop();
        }
    }
}
