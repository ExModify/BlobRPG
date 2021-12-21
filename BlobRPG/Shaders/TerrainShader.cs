using BlobRPG.Entities;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Shaders
{
    public class TerrainShader : ShaderCore
    {

        private int TransformationMatrixLocation;
        private int ProjectionMatrixLocation;
        private int ViewMatrixLocation;
        private int ReflectivityLocation;
        private int ShineDamperLocation;

        private int GradientLocation;
        private int DensityLocation;
        private int FogColor;


        private int LightCountLocation;
        private int[] LightPositionLocation;
        private int[] LightColorLocation;
        private int[] LightAttenuationLocation;

        private int BackgroundTextureLocation;
        private int RTextureLocation;
        private int GTextureLocation;
        private int BTextureLocation;
        private int BlendTextureLocation;
        private int ShadowMapLocation;

        private int ClipPlaneLocation;

        private int ToShadowMapSpaceLocation;
        private int ShadowDistanceLocation;
        private int ShadowMapSizeLocation;
        private int PCFCountLocation;

        public TerrainShader() : base("terrain")
        {

        }

        protected override void BindAttributes()
        {
            BindAttribute(0, "position");
            BindAttribute(1, "textureCoords");
            BindAttribute(2, "normal");
        }


        protected override void GetAllUniformLocations()
        {
            LightPositionLocation = new int[Settings.MAX_LIGHTS];
            LightColorLocation = new int[Settings.MAX_LIGHTS];
            LightAttenuationLocation = new int[Settings.MAX_LIGHTS];

            TransformationMatrixLocation = GetUniformLocation("transformationMatrix");
            ProjectionMatrixLocation = GetUniformLocation("projectionMatrix");
            ViewMatrixLocation = GetUniformLocation("viewMatrix");
            ReflectivityLocation = GetUniformLocation("reflectivity");
            ShineDamperLocation = GetUniformLocation("shineDamper");

            GradientLocation = GetUniformLocation("gradient");
            DensityLocation = GetUniformLocation("density");
            FogColor = GetUniformLocation("fogColor");

            LightCountLocation = GetUniformLocation("lightCount");
            for (int i = 0; i < Settings.MAX_LIGHTS; i++)
            {
                LightPositionLocation[i] = GetUniformLocation("lightPosition[" + i + "]");
                LightColorLocation[i] = GetUniformLocation("lightColor[" + i + "]");
                LightAttenuationLocation[i] = GetUniformLocation("lightAttenuation[" + i + "]");
            }

            BackgroundTextureLocation = GetUniformLocation("backgroundTexture");
            RTextureLocation = GetUniformLocation("rTexture");
            GTextureLocation = GetUniformLocation("gTexture");
            BTextureLocation = GetUniformLocation("bTexture");
            BlendTextureLocation = GetUniformLocation("blendTexture");
            ShadowMapLocation = GetUniformLocation("shadowMap");

            ClipPlaneLocation = GetUniformLocation("clipPlane");
            ToShadowMapSpaceLocation = GetUniformLocation("toShadowMapSpace");
            ShadowDistanceLocation = GetUniformLocation("shadowDistance");
            ShadowMapSizeLocation = GetUniformLocation("shadowMapSize");
            PCFCountLocation = GetUniformLocation("pcfCount");
        }
        public void ConnectTextureUnits()
        {
            LoadInt(BackgroundTextureLocation, 0);
            LoadInt(RTextureLocation, 1);
            LoadInt(GTextureLocation, 2);
            LoadInt(BTextureLocation, 3);
            LoadInt(BlendTextureLocation, 4);
            LoadInt(ShadowMapLocation, 5);
        }

        public void LoadClipPlane(vec4 clipPlane)
        {
            LoadVector(ClipPlaneLocation, clipPlane);
        }
        public void LoadTransformationMatrix(mat4 matrix)
        {
            LoadMatrix(TransformationMatrixLocation, matrix);
        }
        public void LoadProjectionMatrix(mat4 matrix)
        {
            LoadMatrix(ProjectionMatrixLocation, matrix);
        }
        public void LoadViewMatrix(Camera camera)
        {
            LoadMatrix(ViewMatrixLocation, camera.ViewMatrix);
        }
        public void LoadLights(List<Light> lights)
        {
            LoadInt(LightCountLocation, lights.Count);
            int count = Math.Min(lights.Count, Settings.MAX_LIGHTS);
            for (int i = 0; i < count; i++)
            {
                LoadVector(LightPositionLocation[i], lights[i].Position);
                LoadVector(LightColorLocation[i], lights[i].Color);
                LoadVector(LightAttenuationLocation[i], lights[i].Attenuation);
            }
        }

        public void LoadFog(Fog fog)
        {
            LoadFloat(GradientLocation, fog.Gradient);
            LoadFloat(DensityLocation, fog.Density);
            LoadVector(FogColor, fog.FogColor);
        }
        public void LoadShineVariables(float reflectivity, float shineDamper)
        {
            LoadFloat(ReflectivityLocation, reflectivity);
            LoadFloat(ShineDamperLocation, shineDamper);
        }

        public void LoadShadowMapSpace(ref mat4 matrix)
        {
            LoadMatrix(ToShadowMapSpaceLocation, matrix);
        }
        public void LoadShadowVariables(float distance, float size, int pcfCount)
        {
            LoadFloat(ShadowDistanceLocation, distance);
            LoadFloat(ShadowMapSizeLocation, size);
            LoadInt(PCFCountLocation, pcfCount);
        }

    }
}
