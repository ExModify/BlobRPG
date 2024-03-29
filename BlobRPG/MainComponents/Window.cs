﻿using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;
using BlobRPG.Input;
using BlobRPG.Render;
using BlobRPG.Models;
using BlobRPG.Shaders;
using BlobRPG.Textures;
using BlobRPG.Entities;
using BlobRPG.ObjectLoaders;
using GlmSharp;
using System.IO;
using BlobRPG.Particles;
using BlobRPG.SettingsComponents;
using BlobRPG.Render.PostProcessing;
using BlobRPG.Render.PostProcessing.Filters;
using BlobRPG.Audio;
using OpenTK.Audio.OpenAL;

namespace BlobRPG.MainComponents
{
    public class Window : GameWindow
    {

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }

        public Renderer Renderer;
        public Camera Camera;
        public ParticleSystem ParticleSystem;


        Player Player;
        List<Light> Lights;

        List<Entity> Entities;
        List<Terrain> Terrains;
        List<WaterTile> WaterTiles;
        Fog Fog;

        SfxSource TestSource;


        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            Settings.Width = e.Width;
            Settings.Height = e.Height;

            SettingsLoader.SaveSettings();
        }
        protected override void OnMaximized(MaximizedEventArgs e)
        {
            base.OnMaximized(e);

            Settings.WindowState = WindowState;
            SettingsLoader.SaveSettings();
        }
        protected override void OnLoad()
        {
            base.OnLoad();

            GL.Enable(EnableCap.Multisample);

            Settings.InitExtensions();

            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            Settings.AspectRatio = (float)ClientSize.X / ClientSize.Y;

            Fog = new Fog();

            InputManager.Init(this);


            Loader.Init();
            Loader.Load(this);


            /*
            RawModel rm = OBJLoader.LoadSimpleOBJ("starter/model/blob.obj");
            ModelTexture mt = new ModelTexture(Loader.LoadTexture("starter/texture/blobTextureAtlas.png"))
            {
                ShineDamper = 10,
                Reflectivity = 1,
                NumberOfRows = 2
            };
            Player = new Player(new TexturedModel(rm, mt), new vec3(153, 5, -274), textureIndex: 0);
            */
            Player = new Player(AnimatedModel.LoadModel("starter/model/blob.dae", "starter/texture/blobTexture.png", numberOfRows: 1),
                new vec3(153, 5, -274), textureIndex: 0);

            Camera = new Camera(Player);
            Settings.Sun = new Sun(new vec3(1000000, 1500000, -1000000), new vec3(.8f, .8f, .8f), Player);
            Lights = new List<Light>()
            {
                Settings.Sun
            };
            Renderer = new Renderer(this, Camera);

            InputManager.InitMouseRay(this);

            Entities = new List<Entity>();
            Terrains = new List<Terrain>();
            WaterTiles = new List<WaterTile>();

            TerrainTexture backgroundTexture = new TerrainTexture(Loader.LoadTexture("starter/texture/grass.png"));
            TerrainTexture rTexture = new TerrainTexture(Loader.LoadTexture("starter/texture/mud.png"));
            TerrainTexture gTexture = new TerrainTexture(Loader.LoadTexture("starter/texture/grassFlowers.png"));
            TerrainTexture bTexture = new TerrainTexture(Loader.LoadTexture("starter/texture/path.png"));
            TerrainTexture blendTexture = new TerrainTexture(Loader.LoadTexture("starter/texture/blendMap.png"));

            TerrainTexturePack pack = new TerrainTexturePack(backgroundTexture, rTexture, gTexture, bTexture);


            FileStream fs = new FileStream("starter/texture/heightMap.png", FileMode.Open, FileAccess.Read);
            Terrain t = new Terrain(0, -1, pack, blendTexture, fs);
            fs.Close();
            Terrains.Add(t);

            string[] dayTextures = new string[]
            {
                "starter/texture/skybox_day_right.png",
                "starter/texture/skybox_day_left.png",
                "starter/texture/skybox_day_top.png",
                "starter/texture/skybox_day_bottom.png",
                "starter/texture/skybox_day_back.png",
                "starter/texture/skybox_day_front.png"
            };
            string[] nightTextures = new string[]
            {
                "starter/texture/skybox_night_right.png",
                "starter/texture/skybox_night_left.png",
                "starter/texture/skybox_night_top.png",
                "starter/texture/skybox_night_bottom.png",
                "starter/texture/skybox_night_back.png",
                "starter/texture/skybox_night_front.png"
            };
            Stream[] dayStreams = Loader.OpenStreams(dayTextures);
            Stream[] nightStreams = Loader.OpenStreams(nightTextures);

            Renderer.SkyboxRenderer.LoadTextures(dayStreams, nightStreams);

            Loader.CloseStreams(dayStreams);
            Loader.CloseStreams(nightStreams);

            WaterTiles.Add(new WaterTile(176.06757f, -249.94972f, 1.3233751f));


            RawModel barrelModel = OBJLoader.LoadOBJ("starter/model/barrel.obj");
            ModelTexture barrelTexture = new(Loader.LoadTexture("starter/texture/barrelTexture.png"))
            {
                ShineDamper = 10,
                Reflectivity = 1,
                NormalMap = Loader.LoadTexture("starter/texture/barrelNormal.png"),
                SpecularMap = Loader.LoadTexture("starter/texture/barrelGlow.png")
            };
            Entity barrel = new(new TexturedModel(barrelModel, barrelTexture), new vec3(153, 15, -274));

            Entities.Add(barrel);

            RawModel radioModel = OBJLoader.LoadOBJ("starter/model/player.obj");
            ModelTexture radioTexture = new(Loader.LoadTexture("starter/texture/player.png"));
            Entity radio = new(new TexturedModel(radioModel, radioTexture), new vec3(140.9178f, 10.12988f, -294.9091f));
            Entities.Add(radio);
            //Loader.AddText("Meiryo", new vec2(0.0f, 0.5f), "Meiryo");
            //Loader.AddText("Candara", new vec2(0.5f, 0.5f), "Candara");
            //Renderer.AddGUI(new GUITexture(Renderer.ShadowMapTexture, new vec2(0.5f, 0.5f), new vec2(0.5f, 0.5f)));

            ParticleTexture particleTexture = new ParticleTexture(Loader.LoadTexture("starter/texture/smokeParticle.png"), 8);
            ParticleSystem = new ParticleSystem(particleTexture, 1, 0.02f, 0, 700, 1);

            PostProcessor.Init(this);

            if (Settings.PostProcessing)
            {
                //PostProcessor.RegisterFilter(new BrightFilter(this, 0.5f));
                PostProcessor.RegisterFilter(new HorizontalBlurFilter(this, 0.2f)
                {
                    ColorAttachment = 1
                });
                PostProcessor.RegisterFilter(new VerticalBlurFilter(this, 0.2f));
                PostProcessor.RegisterFilter(new CombineFilter(this));
                //PostProcessor.RegisterFilter(new ContrastFilter(this));
            }

            Settings.Sun.CurrentTexture = new ModelTexture(Loader.LoadTexture("starter/texture/sun.png"));


            AudioHandler.Init();
            AudioHandler.SetListenerData(vec3.Zero, new float[6] { 0, 0, 0, 0, 1, 0 });
            AudioHandler.SetDistanceModel();
            TestSource = AudioHandler.LoadSfx("starter/music/music.ogg");
            TestSource.Position = radio.Position;
            TestSource.Loop = true;

            TestSource.SetAttenuation(1, 15, 50);
            //TestSource = AudioHandler.LoadMusic("starter/music/menhera.ogg");
            //TestSource.Loop = true;
        }

        protected override void OnClosed()
        {
            Renderer.CleanUp();
            base.OnClosed();
        }
        bool autoplay = true;
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            Renderer.Clear3DObjects();

            Settings.DeltaTime = args.Time;
            Settings.IngameTime += (float)Settings.DeltaTime * Settings.TimeMultiplier;
            Settings.IngameTime %= Settings.MaxTime;

            if (autoplay)
            {
                autoplay = false;
                if (!TestSource.Playing)
                    TestSource.Play();
            }

            InputManager.Update(this);
            if (InputManager.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.P))
            {
                if (TestSource.Playing)
                    TestSource.Stop();
                else
                    TestSource.Play();
            }
            if (InputManager.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape))
            {
                Close();
            }
            if (InputManager.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Space))
            {
                Console.WriteLine(Player.Position.ToString());
            }
            if (InputManager.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.LeftControl))
            {
                InputManager.ToggleMouse(this);
            }

            Player.Move(Terrains, Entities);
            Player.Update();
            AudioHandler.SetListenerData(Player.Position, Player.Orientation);

            Camera.Move();
            InputManager.UpdateMouseRay();
            Settings.Sun.Update();

            if (Player.Render)
                Renderer.ProcessObject(Player);

            foreach (Entity t in Entities)
                if (t.Model.Texture.NormalMap == -1)
                    Renderer.ProcessObject(t);
                else
                    Renderer.ProcessNormalObject(t);

            foreach (Terrain t in Terrains)
                Renderer.ProcessTerrain(t);

            foreach (WaterTile w in WaterTiles)
                Renderer.ProcessWater(w);

            Renderer.Update();
            ParticleHandler.Update(Camera);

            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            Renderer.Render(Camera, Lights, Fog);
            base.OnRenderFrame(args);
            

            SwapBuffers();
        }

    }
}
