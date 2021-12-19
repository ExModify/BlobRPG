using BlobRPG.Entities;
using BlobRPG.LoggerComponents;
using BlobRPG.SettingsComponents;
using BlobRPG.SettingsComponents.Attributes;
using GlmSharp;
using OpenTK.Windowing.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG
{
    public static class Settings
    {
        /* 
         * Internal variables
         */


        // Player related
        public static double DeltaTime { get; set; } = 0;
        public static double Gravity { get; set; } = -10;
        public static float SkyboxRotation { get; set; } = 1f;

        public const float WalkSpeed = 10;
        public const float RunSpeed = 20;
        public const float JumpHeight = 8;

        public static bool AllowFlight { get; set; } = false;


        // Rendering related
        public const float NEAR = 0.1f;
        public const float FAR = 1000f;


        public static vec3 SkyColor { get; set; } = new vec3(0.529f, 0.807f, 0.921f);
        public static vec3 NightColor { get; set; } = new vec3(0.529f, 0.807f, 0.921f) * 0.5f;

        public static Light Sun { get; set; } = new Light(new vec3(0, 1000, -7000), new vec3(.8f, .8f, .8f));

        
        // Shader variables
        public const int MAX_LIGHTS = 8;
        public const float WAVE_SPEED = 0.03f;


        /* 
         * Configurable settings 
         */
        [Savable("Main")]
        public static LogSeverity LogSeverity { get; set; } = LogSeverity.Debug;

        [Savable("Video"), Min(0)]
        public static int Width { get; set; } = 1280;
        [Savable("Video"), Min(0)]
        public static int Height { get; set; } = 720;
        [Savable("Video"), Min(30)]
        public static int RenderFPS { get; set; } = 240;
        [Savable("Video"), Min(30)]
        public static int UpdateFPS { get; set; } = 240;
        [Savable("Video")]
        public static bool VSync { get; set; } = false;
        [Savable("Video")]
        public static WindowState WindowState { get; set; } = WindowState.Normal;
        [Savable("Video"), Min(40), Max(200)]
        public static int FieldOfView { get; set; } = 70;

        [Savable("Account")]
        public static string Username { get; set; } = "";
        [Savable("Account")]
        public static string Password { get; set; } = "";
    }
}
