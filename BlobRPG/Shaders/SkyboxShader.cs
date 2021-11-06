using BlobRPG.Entities;
using BlobRPG.MainComponents;
using BlobRPG.Render;
using GlmSharp;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Shaders
{
    public class SkyboxShader : ShaderCore
    {
        private int ProjectionMatrixLocation;
        private int ViewMatrixLocation;
        private int FogColorLocation;
        private int CubeMapLocation;
        private int CubeMap2Location;
        private int BlendFactorLocation;

        private int ClipPlaneLocation;

        readonly Window Window;

        float Rotation;

        public SkyboxShader(Window window) : base("skybox")
        {
            Window = window;
        }

        protected override void BindAttributes()
        {
            BindAttribute(0, "position");
        }



        protected override void GetAllUniformLocations()
        {
            ProjectionMatrixLocation = GetUniformLocation("projectionMatrix");
            ViewMatrixLocation = GetUniformLocation("viewMatrix");
            FogColorLocation = GetUniformLocation("fogColor");

            CubeMapLocation = GetUniformLocation("cubeMap");
            CubeMap2Location = GetUniformLocation("cubeMap2");
            BlendFactorLocation = GetUniformLocation("blendFactor");

            ClipPlaneLocation = GetUniformLocation("clipPlane");
        }

        public void LoadClipPlane(vec4 clipPlane)
        {
            LoadVector(ClipPlaneLocation, clipPlane);
        }

        public void LoadProjectionMatrix(ref mat4 matrix)
        {
            LoadMatrix(ProjectionMatrixLocation, matrix);
        }

        public void LoadBlendFactor(float blend)
        {
            LoadFloat(BlendFactorLocation, blend);
        }

        public void ConnectTextureUnits()
        {
            LoadInt(CubeMapLocation, 0);
            LoadInt(CubeMap2Location, 1);
        }
        public void LoadViewMatrix(Camera camera)
        {
            mat4 matrix = camera.ViewMatrix;
            matrix.m30 = 0;
            matrix.m31 = 0;
            matrix.m32 = 0;

            Rotation += (float)(Window.SkyboxRotation * Window.DeltaTime);
            matrix *= mat4.RotateY(MathHelper.DegreesToRadians(Rotation));

            LoadMatrix(ViewMatrixLocation, matrix);
        }

        public void LoadFog(Fog fog)
        {
            LoadVector(FogColorLocation, fog.FogColor);
        }
    }
}
