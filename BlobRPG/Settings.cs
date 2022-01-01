using BlobRPG.Entities;
using BlobRPG.LoggerComponents;
using BlobRPG.SettingsComponents;
using BlobRPG.SettingsComponents.Attributes;
using GlmSharp;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG
{
    public static class Settings
    {
        /* 
         * Internal variables and methods
         */
        private static List<string> Extensions;
        internal static void InitExtensions()
        {
            Extensions = new List<string>();
            int n = GL.GetInteger(GetPName.NumExtensions);
            for (int i = 0; i < n; i++)
            {
                Extensions.Add(GL.GetString(StringNameIndexed.Extensions, i).ToLower());
            }
            if (n == 0)
                Extensions = new List<string>(GL.GetString(StringName.Extensions).ToLower().Split(' '));
        }
        public static bool CheckExtension(string extension)
        {
            return Extensions.Contains(extension.ToLower());
        }

        // Audio related
        public const int AudioBufferCount = 4;
        public const int AudioBufferSize = 1024 * 128;

        // Player related
        public static double DeltaTime { get; set; } = 0;
        public static double Gravity { get; set; } = -10;
        public static float SkyboxRotation { get; set; } = 1f;

        public const float WalkSpeed = 10;
        public const float RunSpeed = 20;
        public const float JumpHeight = 8;
        public const float MaxZoom = 100;
        public const float SunSize = 20f;

        public static bool AllowFlight { get; set; } = false;

        public static StreamWriter LogFile { get; set; }
        public static FileStream LogFileStream { get; set; }
        // Rendering related
        public const float NEAR = 0.1f;
        public const float FAR = 1000f;

        public static vec3 DayColor { get; set; } = new vec3(192, 255, 248) / 255f;
        public static vec3 NightColor { get; set; } = new vec3(3, 61, 113) / 255f;

        public static vec3 DayLight { get; set; } = new vec3(255, 255, 255) / 255f;
        public static vec3 NightLight { get; set; } = new vec3(0, 65, 143) / 255f;
        public static vec3 NoLight { get; set; } = vec3.Zero;
        public static vec3 SkyColor { get; set; } = DayColor;

        public const float TimeMultiplier = 1000;
        public const float MaxTime = 24000;
        public const float PhaseTime = 3000;

        public const float DayPhaseStart = 5000;
        public const float DayPhaseEnd = DayPhaseStart + PhaseTime;
        public const float DayDuration = NightPhaseStart - DayPhaseEnd;

        public const float NightPhaseStart = MaxTime - PhaseTime - DayPhaseStart;
        public const float NightPhaseEnd = MaxTime - DayPhaseStart;
        public const float NightDuration = MaxTime - NightPhaseEnd + DayPhaseStart;

        public static float IngameTime { get; set; } = 0;

        public static Sun Sun { get; set; }

        public static float AspectRatio { get; set; } = Width / (float)Height;


        // Shader variables

        // Animation
        public const int MaxWeights = 3;
        public const int MAX_JOINTS = 50; // change in shader too
        // Lighting - change in shaders too
        public const int MAX_LIGHTS = 8;
        // Water
        public const float WAVE_SPEED = 0.03f;
        // Shadow map
        public const float ShadowDistance = 150;
        public const float ShadowOffset = 10;
        public const int ShadowMapSize = 4096;
        public const int PCFCount = 2;

        /* 
         * Configurable settings 
         */
        [Savable("Main"), Default(LogSeverity.Debug)]
        public static LogSeverity LogSeverity { get; set; } = LogSeverity.Debug;

        [Savable("Video"), Min(0), Default(1280)]
        public static int Width { get; set; } = 1280;
        [Savable("Video"), Min(0), Default(720)]
        public static int Height { get; set; } = 720;
        [Savable("Video"), Min(30), ExcludeValue(0), Default(120)]
        public static int RenderFPS { get; set; } = 120;
        [Savable("Video"), Min(30), ExcludeValue(0), Default(120)]
        public static int UpdateFPS { get; set; } = 120;

        [Savable("Video"), Default(VSyncMode.Off)]
        public static VSyncMode VSync { get; set; } = VSyncMode.Off;

        [Savable("Video"), Default(WindowState.Normal)]
        public static WindowState WindowState { get; set; } = WindowState.Normal;
        [Savable("Video"), Min(40), Max(200), Default(70)]
        public static int FieldOfView { get; set; } = 70;
        [Savable("Video"), Min(0), Max(8), Default(4)]
        public static int MSAA { get; set; } = 4;

        [Savable("Effects"), Default(true)]
        public static bool PostProcessing { get; set; } = true;
        [Savable("Effects"), Min(-1), Max(1), Default(0)]
        public static float Contrast { get; set; } = 0f;

        [Savable("Account")]
        public static string Username { get; set; } = "";
        [Savable("Account")]
        public static string Password { get; set; } = "";
    }
}
