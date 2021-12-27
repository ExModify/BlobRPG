using BlobRPG.Entities;
using BlobRPG.Font;
using BlobRPG.LoggerComponents;
using BlobRPG.MainComponents;
using BlobRPG.Models;
using BlobRPG.Particles;
using BlobRPG.Render.PostProcessing;
using BlobRPG.Render.Shadows;
using BlobRPG.Render.Water;
using BlobRPG.Shaders;
using BlobRPG.Textures;
using GlmSharp;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BlobRPG.Render
{
    public class Renderer : ILogger
    {
        private static readonly vec4 SafetyClipPlane = new(0, -1, 0, 100000000000);

        internal readonly EntityRenderer EntityRenderer;
        internal readonly NormalRenderer NormalRenderer;
        internal readonly TerrainRenderer TerrainRenderer;
        internal readonly GUIRenderer GUIRenderer;
        internal readonly TextRenderer TextRenderer;
        internal readonly SkyboxRenderer SkyboxRenderer;
        internal readonly WaterRenderer WaterRenderer;
        internal readonly ParticleRenderer ParticleRenderer;

        internal readonly ShadowRenderer ShadowRenderer;

        readonly EntityShader EntityShader;
        readonly NormalShader NormalShader;
        readonly TerrainShader TerrainShader;
        readonly GUIShader GUIShader;
        readonly TextShader TextShader;
        readonly SkyboxShader SkyboxShader;
        readonly WaterShader WaterShader;
        readonly ParticleShader ParticleShader;

        readonly Dictionary<TexturedModel, List<Entity>> AllEntities;

        readonly Dictionary<TexturedModel, List<Entity>> Entities;
        readonly Dictionary<TexturedModel, List<Entity>> NormalEntities;

        readonly Dictionary<FontType, List<GUIText>> Texts;
        readonly List<WaterTile> WaterTiles;
        readonly List<Terrain> Terrains;
        readonly List<GUITexture> GUIs;

        readonly WaterFrameBuffers WaterFrameBuffers;

        readonly Stopwatch Stopwatch;

        public int ShadowMapTexture
        {
            get
            {
                return ShadowRenderer.ShadowMap;
            }
        }

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

        public Renderer(Window window, Camera camera)
        {
            AllEntities = new Dictionary<TexturedModel, List<Entity>>();
            Entities = new Dictionary<TexturedModel, List<Entity>>();
            NormalEntities = new Dictionary<TexturedModel, List<Entity>>();
            Terrains = new List<Terrain>();
            GUIs = new List<GUITexture>();
            Texts = new Dictionary<FontType, List<GUIText>>();
            WaterTiles = new List<WaterTile>();

            EnableCulling();

            window.Resize += (s) =>
            {
                Settings.AspectRatio = (float)s.Width / s.Height;
                CreateProjectionMatrix(Settings.FieldOfView, s.Width, s.Height, Settings.NEAR, Settings.FAR);
                UpdateProjectionMatrix();

            };


            CreateProjectionMatrix(Settings.FieldOfView, window.ClientSize.X, window.ClientSize.Y, Settings.NEAR, Settings.FAR);

            EntityShader = new EntityShader();
            EntityRenderer = new EntityRenderer(EntityShader, ref ProjectionMatrix);

            NormalShader = new NormalShader();
            NormalRenderer = new NormalRenderer(NormalShader, ref ProjectionMatrix);

            TerrainShader = new TerrainShader();
            TerrainRenderer = new TerrainRenderer(TerrainShader, ref ProjectionMatrix);

            GUIShader = new GUIShader();
            GUIRenderer = new GUIRenderer(GUIShader);

            TextShader = new TextShader();
            TextRenderer = new TextRenderer(TextShader);

            SkyboxShader = new SkyboxShader();
            SkyboxRenderer = new SkyboxRenderer(SkyboxShader, ref ProjectionMatrix);

            ParticleShader = new ParticleShader();
            ParticleRenderer = new ParticleRenderer(ParticleShader, ref ProjectionMatrix);

            ShadowRenderer = new ShadowRenderer(camera);


            WaterFrameBuffers = new WaterFrameBuffers(window);
            int waterDUDVTexture = Loader.LoadTexture("starter/texture/wassa.png");
            int waterNormalTexture = Loader.LoadTexture("starter/texture/wassaNormal.png");

            WaterShader = new WaterShader();
            WaterRenderer = new WaterRenderer(WaterShader, ref ProjectionMatrix, WaterFrameBuffers, waterDUDVTexture, waterNormalTexture);

            Stopwatch = new();
        }
        public void Update()
        {
            SkyboxRenderer.Update();
        }

        public void Render(Camera camera, List<Light> lights, Fog fog, bool debug = false)
        {
            if (debug)
            {
                RenderDebug(camera, lights, fog);
                return;
            }
            RenderShadowMap(lights[0]);

            GL.Enable(EnableCap.ClipDistance0);

            for (int i = 0; i < WaterTiles.Count; i++)
            {
                WaterFrameBuffers.BindReflectionFB();
                camera.MoveUnderWaterTile(WaterTiles[i]);
                Render3DObjects(camera, lights, fog, WaterTiles[i].ReflectionClipPlane);
                camera.RevertWaterTileMove();

                WaterFrameBuffers.BindRefractionFB();

                Render3DObjects(camera, lights, fog, WaterTiles[i].RefractionClipPlane);
            }

            GL.Disable(EnableCap.ClipDistance0);

            PostProcessor.StartSceneRendering();

            Render3DObjects(camera, lights, fog, SafetyClipPlane);
            for (int i = 0; i < WaterTiles.Count; i++)
                WaterRenderer.Render(WaterTiles[i], camera, Settings.Sun);

            ParticleRenderer.Render(ParticleHandler.Particles, camera);

            PostProcessor.FinishSceneRendering();

            GUIRenderer.Render(GUIs);
            TextRenderer.Render(Texts);

            Terrains.Clear();
            Entities.Clear();
            NormalEntities.Clear();
            WaterTiles.Clear();
        }
        public void RenderDebug(Camera camera, List<Light> lights, Fog fog)
        {
            Log(Debug, "Starting rendering");
            Stopwatch.Start();
            RenderShadowMap(lights[0]);
            Log(Debug, $"Rendered shadow map: { Stopwatch.ElapsedTicks } ticks, { Stopwatch.ElapsedMilliseconds }ms");
            Stopwatch.Restart();

            GL.Enable(EnableCap.ClipDistance0);
            Log(Debug, $"Enabling clip distance: { Stopwatch.ElapsedTicks } ticks, { Stopwatch.ElapsedMilliseconds }ms");
            Stopwatch.Restart();

            for (int i = 0; i < WaterTiles.Count; i++)
            {
                WaterFrameBuffers.BindReflectionFB();
                Log(Debug, $"Reflection rendered: { Stopwatch.ElapsedTicks } ticks, { Stopwatch.ElapsedMilliseconds }ms");
                Stopwatch.Restart();
                camera.MoveUnderWaterTile(WaterTiles[i]);
                Log(Debug, $"Camera moved: { Stopwatch.ElapsedTicks } ticks, { Stopwatch.ElapsedMilliseconds }ms");
                Stopwatch.Restart();
                Render3DObjects(camera, lights, fog, WaterTiles[i].ReflectionClipPlane);
                Log(Debug, $"Refraction rendered: { Stopwatch.ElapsedTicks } ticks, { Stopwatch.ElapsedMilliseconds }ms");
                Stopwatch.Restart();
                camera.RevertWaterTileMove();
                Log(Debug, $"Camera reverted: { Stopwatch.ElapsedTicks } ticks, { Stopwatch.ElapsedMilliseconds }ms");
                Stopwatch.Restart();

                WaterFrameBuffers.BindRefractionFB();
                Log(Debug, $"Bound refraction: { Stopwatch.ElapsedTicks } ticks, { Stopwatch.ElapsedMilliseconds }ms");
                Stopwatch.Restart();

                Render3DObjects(camera, lights, fog, WaterTiles[i].RefractionClipPlane);
                Log(Debug, $"Rendering refraction: { Stopwatch.ElapsedTicks } ticks, { Stopwatch.ElapsedMilliseconds }ms");
                Stopwatch.Restart();
            }

            GL.Disable(EnableCap.ClipDistance0);
            Log(Debug, $"Disabling clip distance: { Stopwatch.ElapsedTicks } ticks, { Stopwatch.ElapsedMilliseconds }ms");
            Stopwatch.Restart();

            PostProcessor.StartSceneRendering();
            Log(Debug, $"Binding post processing FBO: { Stopwatch.ElapsedTicks } ticks, { Stopwatch.ElapsedMilliseconds }ms");
            Stopwatch.Restart();

            Render3DObjects(camera, lights, fog, SafetyClipPlane);
            Log(Debug, $"Main 3D rendering: { Stopwatch.ElapsedTicks } ticks, { Stopwatch.ElapsedMilliseconds }ms");
            Stopwatch.Restart();
            for (int i = 0; i < WaterTiles.Count; i++)
                WaterRenderer.Render(WaterTiles[i], camera, Settings.Sun);

            Log(Debug, $"Rendered water: { Stopwatch.ElapsedTicks } ticks, { Stopwatch.ElapsedMilliseconds }ms");
            Stopwatch.Restart();

            ParticleRenderer.Render(ParticleHandler.Particles, camera);

            Log(Debug, $"Rendered particles: { Stopwatch.ElapsedTicks } ticks, { Stopwatch.ElapsedMilliseconds }ms");
            Stopwatch.Restart();

            PostProcessor.FinishSceneRendering();

            Log(Debug, $"Post processing: { Stopwatch.ElapsedTicks } ticks, { Stopwatch.ElapsedMilliseconds }ms");
            Stopwatch.Restart();

            GUIRenderer.Render(GUIs);
            TextRenderer.Render(Texts);

            Terrains.Clear();
            Entities.Clear();
            NormalEntities.Clear();
            WaterTiles.Clear();

            Log(Debug, $"Rendering text, GUI and clearing lists: { Stopwatch.ElapsedTicks } ticks, { Stopwatch.ElapsedMilliseconds }ms");
            Stopwatch.Stop();
            Log(Debug, "--------------");
        }
        private void Prepare()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Settings.SkyColor.x, Settings.SkyColor.y, Settings.SkyColor.z, 1);

            GL.ActiveTexture(TextureUnit.Texture5);
            GL.BindTexture(TextureTarget.Texture2D, ShadowMapTexture);
        }
        private void Render3DObjects(Camera camera, List<Light> lights, Fog fog, vec4 clipPlane)
        {
            Prepare();
            mat4 toShadowSpace = ShadowRenderer.ToShadowMapSpaceMatrix;

            EntityRenderer.Render(Entities, camera, lights, fog, clipPlane, ref toShadowSpace);
            NormalRenderer.Render(NormalEntities, camera, lights, fog, clipPlane, ref toShadowSpace);
            TerrainRenderer.Render(Terrains, camera, lights, fog, clipPlane, ref toShadowSpace);
            SkyboxRenderer.Render(camera, fog, clipPlane);
        }
        private void RenderShadowMap(Light sun)
        {
            ShadowRenderer.Render(AllEntities, sun);
            AllEntities.Clear();
        }

        public void CleanUp()
        {
            WaterFrameBuffers.CleanUp();
            EntityShader.CleanUp();
            NormalShader.CleanUp();
            TerrainShader.CleanUp();
            GUIShader.CleanUp();
            TextShader.CleanUp();
            SkyboxShader.CleanUp();
            WaterShader.CleanUp();
            ShadowRenderer.CleanUp();
        }

        public void AddGUI(GUITexture texture)
        {
            GUIs.Add(texture);
        }
        public void RemoveGUI(GUITexture texture)
        {
            GUIs.Remove(texture);
        }

        public void AddText(GUIText text)
        {
            if (Texts.Keys.Contains(text.Font))
            {
                Texts[text.Font].Add(text);
            }
            else
            {
                Texts.Add(text.Font, new List<GUIText>() { text });
            }
        }
        public void RemoveText(GUIText text)
        {
            Texts[text.Font].Remove(text);
            if (Texts[text.Font].Count == 0)
                Texts.Remove(text.Font);
        }

        public void ProcessWater(WaterTile tile)
        {
            WaterTiles.Add(tile);
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
                AllEntities[entity.Model].Add(entity);
            }
            else
            {
                Entities.Add(entity.Model, new List<Entity>() { entity });
                AllEntities.Add(entity.Model, new List<Entity>() { entity });
            }
        }
        public void ProcessNormalObject(Entity entity)
        {
            if (NormalEntities.Keys.Contains(entity.Model))
            {
                NormalEntities[entity.Model].Add(entity);
                AllEntities[entity.Model].Add(entity);
            }
            else
            {
                NormalEntities.Add(entity.Model, new List<Entity>() { entity });
                AllEntities.Add(entity.Model, new List<Entity>() { entity });
            }
        }



        private void CreateProjectionMatrix(float fov, int width, int height, float near, float far)
        {
            //ProjectionMatrix = mat4.PerspectiveFov(fov, width, height, near, far);

            ProjectionMatrix = mat4.Identity;
            float aspectRatio = (float)width / (float)height;
            float y_scale = (float)((1f / Math.Tan(MathHelper.DegreesToRadians(fov/ 2f))));
            float x_scale = y_scale / aspectRatio;
            float frustum_length = far - near;

            ProjectionMatrix.m00 = x_scale;
            ProjectionMatrix.m11 = y_scale;
            ProjectionMatrix.m22 = -((far + near) / frustum_length);
            ProjectionMatrix.m23 = -1;
            ProjectionMatrix.m32 = -((2 * near * far) / frustum_length);
            ProjectionMatrix.m33 = 0;
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

            WaterShader.Start();
            WaterShader.LoadProjectionMatrix(ref ProjectionMatrix);
            WaterShader.LoadPlaneVariables(Settings.NEAR, Settings.FAR);
            WaterShader.Stop();
        }
    }
}
