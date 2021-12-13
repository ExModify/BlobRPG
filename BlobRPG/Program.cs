using BlobRPG.LoggerComponents;
using BlobRPG.MainComponents;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;

namespace BlobRPG
{
    public class Program
    {
        /* Change this variable in the shaders as well */
        public const int MAX_LIGHTS = 8;
        public const float WAVE_SPEED = 0.03f;
        public static LogSeverity LogSeverity { get; set; } = LogSeverity.Debug;

        static void Main(string[] args)
        {
            GameWindowSettings gameWindowSettings = new()
            {
                RenderFrequency = 240,
                UpdateFrequency = 240
            };
            NativeWindowSettings nativeWindowSettings = new()
            {
                API = ContextAPI.OpenGL,
                APIVersion = new Version(3, 2),
                AutoLoadBindings = true,
                Flags = ContextFlags.Default,
                Profile = ContextProfile.Core,
                Size = new OpenTK.Mathematics.Vector2i(1280, 720),
                Title = "BlobRPG"
            };

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                nativeWindowSettings.Flags |= ContextFlags.ForwardCompatible;

            using Window game = new(gameWindowSettings, nativeWindowSettings);
            game.Run();

            Loader.CleanUp();
        }
    }
}
