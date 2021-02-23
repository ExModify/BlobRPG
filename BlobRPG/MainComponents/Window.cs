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

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }

        Renderer Renderer;

        Entity Entity;
        Light Light;

        Camera Camera;

        Terrain Terrain;
        Fog Fog;

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);

            SkyColor = new vec3(0.529f, 0.807f, 0.921f);
            Fog = new Fog(this);

            InputManager.Init(this);

            Loader.Init();
            Loader.Load();

            Renderer = new Renderer(this);

            RawModel rm = OBJLoader.LoadOBJ("starter/model/blob.obj");
            ModelTexture mt = new ModelTexture(Loader.LoadTexture("starter/texture/blobTexture.png"))
            {
                ShineDamper = 10,
                Reflectivity = 1
            };
            Entity = new Entity(new TexturedModel(rm, mt), new GlmSharp.vec3(0, 0, -4));

            Camera = new Camera(new GlmSharp.vec3(0, 1, 0), 0, 0, 0);
            Light = new Light(new GlmSharp.vec3(0, 0, 20), new GlmSharp.vec3(1, 1, 1));


            TerrainTexture backgroundTexture = new TerrainTexture(Loader.LoadTexture("starter/texture/grass.png"));
            TerrainTexture rTexture = new TerrainTexture(Loader.LoadTexture("starter/texture/mud.png"));
            TerrainTexture gTexture = new TerrainTexture(Loader.LoadTexture("starter/texture/grassFlowers.png"));
            TerrainTexture bTexture = new TerrainTexture(Loader.LoadTexture("starter/texture/path.png"));
            TerrainTexture blendTexture = new TerrainTexture(Loader.LoadTexture("starter/texture/blendMap.png"));

            TerrainTexturePack pack = new TerrainTexturePack(backgroundTexture, rTexture, gTexture, bTexture);

            FileStream fs = new FileStream("starter/texture/heightMap.png", FileMode.Open, FileAccess.Read);
            Terrain = new Terrain(-1, -1, pack, blendTexture, fs);
            fs.Close();
        }

        protected override void OnClosed()
        {
            Renderer.CleanUp();
            base.OnClosed();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            InputManager.Update(this);

            if (InputManager.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape))
            {
                Close();
            }
            if (InputManager.IsKeyPressed(OpenTK.Windowing.GraphicsLibraryFramework.Keys.LeftControl))
            {
                InputManager.ToggleMouse(this);
            }

            Camera.Move();

            Renderer.ProcessObject(Entity);
            Renderer.ProcessTerrain(Terrain);

            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            Renderer.Render(Camera, Light, Fog);

            SwapBuffers();
            base.OnRenderFrame(args);
        }
    }
}
