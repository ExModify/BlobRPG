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

namespace BlobRPG.MainComponents
{
    public class Window : GameWindow
    {
        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }

        EntityRenderer entityRenderer;
        EntityShader entityShader;
        RawModel model;

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            InputManager.Init(this);
            Loader.Init();
            Loader.Load();
            entityRenderer = new EntityRenderer();
            entityShader = new EntityShader();
            model = Loader.LoadToVao(new float[]
            {
                 0.5f,  0.5f, 0.0f,
                 0.5f, -0.5f, 0.0f,
                -0.5f, -0.5f, 0.0f,
                -0.5f,  0.5f, 0.0f,
            }, new int[]
            {
                0, 1, 3, 
                1, 2, 3 
            });

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

            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            entityRenderer.Prepare();
            entityShader.Start();
            entityRenderer.Render(model);
            entityShader.Stop();
            SwapBuffers();
            base.OnRenderFrame(args);
        }
    }
}
