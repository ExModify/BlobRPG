using BlobRPG.LoggerComponents;
using BlobRPG.MainComponents;
using BlobRPG.SettingsComponents;
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
        private static Window Game;
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            SettingsLoader.LoadSettings();

            GameWindowSettings gameWindowSettings = new()
            {
                RenderFrequency = Settings.RenderFPS,
                UpdateFrequency = Settings.UpdateFPS
            };
            NativeWindowSettings nativeWindowSettings = new()
            {
                API = ContextAPI.OpenGL,
                APIVersion = new Version(3, 2),
                AutoLoadBindings = true,
                Flags = ContextFlags.Default,
                Profile = ContextProfile.Core,
                Size = new OpenTK.Mathematics.Vector2i(Settings.Width, Settings.Height),
                WindowState = Settings.WindowState,
                Title = "BlobRPG"
            };


            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                nativeWindowSettings.Flags |= ContextFlags.ForwardCompatible;

            Game = new(gameWindowSettings, nativeWindowSettings);
            Game.Run();
            Game.Dispose();

            Loader.CleanUp();
        }

        public static void Halt()
        {
            Game.Close();
            Environment.Exit(-1);
        }
    }
}
