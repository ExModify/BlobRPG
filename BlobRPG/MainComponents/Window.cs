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

namespace BlobRPG.MainComponents
{
    public class Window : GameWindow
    {
        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }

        Renderer Renderer;

        Entity Entity;
        Light Light;

        Camera Camera;

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);

            InputManager.Init(this);

            Loader.Init();
            Loader.Load();

            Renderer = new Renderer(this);

            RawModel rm = OBJLoader.LoadOBJ("starter/model/blob.obj");
            ModelTexture mt = new ModelTexture(Loader.LoadTexture("starter/texture/blobTexture.png"));
            Entity = new Entity(new TexturedModel(rm, mt), new GlmSharp.vec3(0, 0, -4));

            Camera = new Camera(new GlmSharp.vec3(), 0, 0, 0);
            Light = new Light(new GlmSharp.vec3(0, 0, 20), new GlmSharp.vec3(1, 1, 1));
            
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


            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            Renderer.Render(Camera, Light);

            SwapBuffers();
            base.OnRenderFrame(args);
        }
    }
}
