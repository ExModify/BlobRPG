﻿using BlobRPG.Entities;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Shaders
{
    public class NormalShader : ShaderCore
    {
        private int TransformationMatrixLocation;
        private int ProjectionMatrixLocation;
        private int ViewMatrixLocation;
        private int ReflectivityLocation;
        private int ShineDamperLocation;
        private int UseFakeLightingLocation;
        private int UseSpecularMapLocation;


        private int TextureSamplerLocation;
        private int NormalMapLocation;
        private int SpecularMapLocation;

        private int ClipPlaneLocation;

        private int GradientLocation;
        private int DensityLocation;
        private int FogColor;

        private int LightCountLocation;
        private int[] LightPositionLocation;
        private int[] LightColorLocation;
        private int[] LightAttenuationLocation;

        private int TextureOffsetLocation;
        private int NumberOfRowsLocation;

        private int ShadowMapLocation;
        private int ToShadowMapSpaceLocation;
        private int ShadowDistanceLocation;
        private int ShadowMapSizeLocation;
        private int PCFCountLocation;

        public NormalShader() : base("normal")
        {

        }

        protected override void BindAttributes()
        {
            BindAttribute(0, "position");
            BindAttribute(1, "textureCoords");
            BindAttribute(2, "normal");
            BindAttribute(3, "tangent");
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
            UseFakeLightingLocation = GetUniformLocation("useFakeLighting");
            UseSpecularMapLocation = GetUniformLocation("useSpecularMap");

            TextureSamplerLocation = GetUniformLocation("textureSampler");
            NormalMapLocation = GetUniformLocation("normalMap");
            SpecularMapLocation = GetUniformLocation("specularMap");

            GradientLocation = GetUniformLocation("gradient");
            DensityLocation = GetUniformLocation("density");
            FogColor = GetUniformLocation("fogColor");

            LightCountLocation = GetUniformLocation("lightCount");
            for (int i = 0; i < Settings.MAX_LIGHTS; i++)
            {
                LightPositionLocation[i] = GetUniformLocation("lightPositionEyeSpace[" + i + "]");
                LightColorLocation[i] = GetUniformLocation("lightColor[" + i + "]");
                LightAttenuationLocation[i] = GetUniformLocation("lightAttenuation[" + i + "]");
            }

            TextureOffsetLocation = GetUniformLocation("textureOffset");
            NumberOfRowsLocation = GetUniformLocation("numberOfRows");

            ClipPlaneLocation = GetUniformLocation("clipPlane");

            ToShadowMapSpaceLocation = GetUniformLocation("toShadowMapSpace");
            ShadowDistanceLocation = GetUniformLocation("shadowDistance");
            ShadowMapSizeLocation = GetUniformLocation("shadowMapSize");
            PCFCountLocation = GetUniformLocation("pcfCount");
            ShadowMapLocation = GetUniformLocation("shadowMap");
        }
        public void ConnectTextureUnits()
        {
            LoadInt(TextureSamplerLocation, 0);
            LoadInt(NormalMapLocation, 1);
            LoadInt(ShadowMapLocation, 5);
            LoadInt(SpecularMapLocation, 6);
        }
        public void LoadClipPlane(vec4 clipPlane)
        {
            LoadVector(ClipPlaneLocation, clipPlane);
        }
        public void LoadNumberOfRows(int numberOfRows)
        {
            LoadInt(NumberOfRowsLocation, numberOfRows);
        }
        public void LoadTextureOffset(vec2 textureOffset)
        {
            LoadVector(TextureOffsetLocation, textureOffset);
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
        public void LoadLights(List<Light> lights, Camera camera)
        {
            LoadInt(LightCountLocation, lights.Count);
            int count = Math.Min(lights.Count, Settings.MAX_LIGHTS);
            for (int i = 0; i < count; i++)
            {
                LoadVector(LightPositionLocation[i], new vec3(camera.ViewMatrix * new vec4(lights[i].Position, 1f)));
                LoadVector(LightColorLocation[i], lights[i].Color);
                LoadVector(LightAttenuationLocation[i], lights[i].Attenuation);
            }
        }
        public void LoadFakeLighting(bool fakeLighting)
        {
            LoadBool(UseFakeLightingLocation, fakeLighting);
        }
        public void LoadSpecularMap(bool specularMap)
        {
            LoadBool(UseSpecularMapLocation, specularMap);
        }
        public void HasSpecularMap(bool specularMap)
        {
            LoadBool(UseFakeLightingLocation, specularMap);
        }
        public void LoadShineVariables(float reflectivity, float shineDamper)
        {
            LoadFloat(ReflectivityLocation, reflectivity);
            LoadFloat(ShineDamperLocation, shineDamper);
        }
        public void LoadFog(Fog fog)
        {
            LoadFloat(GradientLocation, fog.Gradient);
            LoadFloat(DensityLocation, fog.Density);
            LoadVector(FogColor, fog.FogColor);
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
