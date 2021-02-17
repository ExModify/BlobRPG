using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;
using BlobRPG.Input;

namespace BlobRPG.Window
{
    public class Game : GameWindow
    {

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings) { }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            InputManager.Init(this);
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
            base.OnRenderFrame(args);
        }
    }
}
