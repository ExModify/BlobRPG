using BlobRPG.Entities;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Shaders
{
    public class WaterShader : ShaderCore
    {
        private int CameraPositionLocation;

        private int TransformationMatrixLocation;
        private int ViewMatrixLocation;
        private int ProjectionMatrixLocation;

        private int ReflectionTextureLocation;
        private int RefractionTextureLocation;
        private int DepthTextureLocation;
        private int DUDVTextureLocation;
        private int NormalTextureLocation;

        private int MoveFactorLocation;
        private int WaveStrengthLocation;
        private int TilingLocation;

        private int NearPlaneLocation;
        private int FarPlaneLocation;

        private int LightColorLocation;
        private int LightPositionLocation;

        private int ShineDamperLocation;
        private int ReflectivityLocation;

        public WaterShader() : base("water")
        {

        }
        protected override void BindAttributes()
        {
            BindAttribute(0, "position");
        }
        public void ConnectTextureUnits()
        {
            LoadInt(ReflectionTextureLocation, 0);
            LoadInt(RefractionTextureLocation, 1);
            LoadInt(DUDVTextureLocation, 2);
            LoadInt(NormalTextureLocation, 3);
            LoadInt(DepthTextureLocation, 4);
        }
        protected override void GetAllUniformLocations()
        {
            CameraPositionLocation = GetUniformLocation("cameraPosition");

            TransformationMatrixLocation = GetUniformLocation("transformationMatrix");
            ViewMatrixLocation = GetUniformLocation("viewMatrix");
            ProjectionMatrixLocation = GetUniformLocation("projectionMatrix");

            ReflectionTextureLocation = GetUniformLocation("reflectionTexture");
            RefractionTextureLocation = GetUniformLocation("refractionTexture");
            DepthTextureLocation = GetUniformLocation("depthTexture");
            DUDVTextureLocation = GetUniformLocation("dudvTexture");
            NormalTextureLocation = GetUniformLocation("normalTexture");

            MoveFactorLocation = GetUniformLocation("moveFactor");
            WaveStrengthLocation = GetUniformLocation("waveStrength");
            TilingLocation = GetUniformLocation("tiling");

            LightColorLocation = GetUniformLocation("lightColor");
            LightPositionLocation = GetUniformLocation("lightPosition");

            ShineDamperLocation = GetUniformLocation("shineDamper");
            ReflectivityLocation = GetUniformLocation("reflectivity");

            NearPlaneLocation = GetUniformLocation("nearPlane");
            FarPlaneLocation = GetUniformLocation("farPlane");
        }
        public void LoadPlaneVariables(float near, float far)
        {
            LoadFloat(NearPlaneLocation, near);
            LoadFloat(FarPlaneLocation, far);
        }
        public void LoadShineVariables(float shineDamper, float reflectivity)
        {
            LoadFloat(ShineDamperLocation, shineDamper);
            LoadFloat(ReflectivityLocation, reflectivity);
        }
        public void LoadSun(Light light)
        {
            LoadVector(LightColorLocation, light.Color);
            LoadVector(LightPositionLocation, light.Position);
        }
        public void LoadWaveStrength(float strength)
        {
            LoadFloat(WaveStrengthLocation, strength);
        }
        public void LoadMoveFactor(float moveFactor)
        {
            LoadFloat(MoveFactorLocation, moveFactor);
        }
        public void LoadTiling(float tiling)
        {
            LoadFloat(TilingLocation, tiling);
        }
        public void LoadTransformationMatrix(mat4 matrix)
        {
            LoadMatrix(TransformationMatrixLocation, matrix);
        }
        public void LoadViewMatrix(Camera camera)
        {
            LoadMatrix(ViewMatrixLocation, camera.ViewMatrix);
            LoadVector(CameraPositionLocation, camera.Position);
        }
        public void LoadProjectionMatrix(ref mat4 matrix)
        {
            LoadMatrix(ProjectionMatrixLocation, matrix);
        }
    }
}
