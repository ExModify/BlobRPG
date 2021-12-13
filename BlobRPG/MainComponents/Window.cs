using OpenTK.Windowing.Common;
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
using BlobRPG.ObjectManager;
using GlmSharp;
using System.IO;

namespace BlobRPG.MainComponents
{
    public class Window : GameWindow
    {
        public vec3 SkyColor;
        public vec3 NightColor;

        public double DeltaTime { get; private set; } = 0;
        public double Gravity { get; private set; } = -10;
        public float SkyboxRotation { get; private set; } = 1f;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }

        public Renderer Renderer;
        public Camera Camera;

        Player Player;
        List<Light> Lights;

        List<Entity> Entities;
        List<Entity> NormalEntities;
        List<Terrain> Terrains;
        List<WaterTile> WaterTiles;
        Fog Fog;

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);

            SkyColor = new vec3(0.529f, 0.807f, 0.921f);
            NightColor = new vec3(SkyColor * 0.5f);

            Fog = new Fog(this);

            InputManager.Init(this);

            Loader.Init();
            Loader.Load(this);

            Renderer = new Renderer(this);

            RawModel rm = OBJLoader.LoadSimpleOBJ("starter/model/blob.obj");
            ModelTexture mt = new ModelTexture(Loader.LoadTexture("starter/texture/blobTextureAtlas.png"))
            {
                ShineDamper = 10,
                Reflectivity = 1,
                NumberOfRows = 2
            };
            Player = new Player(new TexturedModel(rm, mt), new vec3(153, 5, -274), this, textureIndex: 0);

            Camera = new Camera(Player, this);
            Lights = new List<Light>()
            {
                new Light(new vec3(0, 1000, -7000), new vec3(.8f, .8f, .8f)),
            };

            InputManager.InitMouseRay(this);

            Entities = new List<Entity>();
            NormalEntities = new List<Entity>();
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
                NormalMap = Loader.LoadTexture("starter/texture/barrelNormal.png")
            };
            Entity barrel = new(new TexturedModel(barrelModel, barrelTexture), new vec3(153, 15, -274));

            NormalEntities.Add(barrel);
            Loader.AddText("Meiryo", new vec2(0.0f, 0.5f), "Meiryo");
            Loader.AddText("Candara", new vec2(0.5f, 0.5f), "Candara");
            //Renderer.AddGUI(new GUITexture(Loader.LoadTexture("starter/texture/grass.png"), new vec2(0.5f, 0.5f), new vec2(0.25f, 0.25f)));
        }

        protected override void OnClosed()
        {
            Renderer.CleanUp();
            base.OnClosed();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            DeltaTime = args.Time;

            InputManager.Update(this);

            if (InputManager.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape))
            {
                Close();
            }
            if (InputManager.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.LeftControl))
            {
                InputManager.ToggleMouse(this);
            }

            Player.Move(Terrains);
            
            Camera.Move();
            InputManager.UpdateMouseRay();

            if (Player.Render)
                Renderer.ProcessObject(Player);

            foreach (Entity t in Entities)
                Renderer.ProcessObject(t);

            foreach (Entity t in NormalEntities)
                Renderer.ProcessNormalObject(t);

            foreach (Terrain t in Terrains)
                Renderer.ProcessTerrain(t);

            foreach (WaterTile w in WaterTiles)
                Renderer.ProcessWater(w);

            Renderer.Update();

            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            Renderer.Render(Camera, Lights, Fog);

            SwapBuffers();
            base.OnRenderFrame(args);
        }

    }
}
