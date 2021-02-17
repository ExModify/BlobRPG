using BlobRPG.Window;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Runtime.InteropServices;

namespace BlobRPG
{
    class Program
    {
        static void Main(string[] args)
        {
            GameWindowSettings gameWindowSettings = new GameWindowSettings()
            {
                RenderFrequency = 240,
                UpdateFrequency = 240
            };
            NativeWindowSettings nativeWindowSettings = new NativeWindowSettings()
            {
                API = ContextAPI.OpenGL,
                APIVersion = new Version(3, 2),
                AutoLoadBindings = true,
                Flags = ContextFlags.Default,
                Profile = ContextProfile.Core,
                Size = new OpenTK.Mathematics.Vector2i(1280, 720),
                Title = "BlobRPG"
            };
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                nativeWindowSettings.Flags |= ContextFlags.ForwardCompatible;

            using Game game = new Game(gameWindowSettings, nativeWindowSettings);
            game.Run();
        }
    }
}
