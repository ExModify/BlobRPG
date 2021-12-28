using BlobRPG.Entities;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Shaders
{
    public class EntityShader : ShaderCore
    {
        private int TransformationMatrixLocation;
        private int ProjectionMatrixLocation;
        private int ViewMatrixLocation;
        private int ReflectivityLocation;
        private int ShineDamperLocation;
        private int UseFakeLightingLocation;
        private int UseSpecularMapLocation;

        private int TextureSamplerLocation;
        private int SpecularMapLocation;

        private int GradientLocation;
        private int DensityLocation;
        private int FogColor;

        private int LightCountLocation;
        private int[] LightPositionLocation;
        private int[] LightColorLocation;
        private int[] LightAttenuationLocation;

        private int[] JointTransformsLocation;
        private int JointTransformCountLocation;

        private int TextureOffsetLocation;
        private int NumberOfRowsLocation;

        private int ClipPlaneLocation;

        private int ToShadowMapSpaceLocation;
        private int ShadowDistanceLocation;
        private int ShadowMapSizeLocation;
        private int PCFCountLocation;
        private int ShadowMapLocation;

        public EntityShader() : base("entity")
        {
            
        }

        protected override void BindAttributes()
        {
            BindAttribute(0, "position");
            BindAttribute(1, "textureCoords");
            BindAttribute(2, "normal");
            BindAttribute(3, "tangent");
            BindAttribute(4, "jointIndices");
            BindAttribute(5, "weights");
        }
        public void ConnectTextureUnits()
        {
            LoadInt(TextureSamplerLocation, 0);
            LoadInt(ShadowMapLocation, 5);
            LoadInt(SpecularMapLocation, 6);
        }

        protected override void GetAllUniformLocations()
        {
            LightPositionLocation = new int[Settings.MAX_LIGHTS];
            LightColorLocation = new int[Settings.MAX_LIGHTS];
            LightAttenuationLocation = new int[Settings.MAX_LIGHTS];
            JointTransformsLocation = new int[Settings.MAX_JOINTS];

            TransformationMatrixLocation = GetUniformLocation("transformationMatrix");
            ProjectionMatrixLocation = GetUniformLocation("projectionMatrix");
            ViewMatrixLocation = GetUniformLocation("viewMatrix");
            ReflectivityLocation = GetUniformLocation("reflectivity");
            ShineDamperLocation = GetUniformLocation("shineDamper");
            UseFakeLightingLocation = GetUniformLocation("useFakeLighting");
            UseSpecularMapLocation = GetUniformLocation("useSpecularMap");

            TextureSamplerLocation = GetUniformLocation("textureSampler");
            SpecularMapLocation = GetUniformLocation("specularMap");

            GradientLocation = GetUniformLocation("gradient");
            DensityLocation = GetUniformLocation("density");
            FogColor = GetUniformLocation("fogColor");

            LightCountLocation = GetUniformLocation("lightCount");
            JointTransformCountLocation = GetUniformLocation("jointTransformCount");

            for (int i = 0; i < Settings.MAX_LIGHTS; i++)
            {
                LightPositionLocation[i] = GetUniformLocation("lightPosition[" + i + "]");
                LightColorLocation[i] = GetUniformLocation("lightColor[" + i + "]");
                LightAttenuationLocation[i] = GetUniformLocation("lightAttenuation[" + i + "]");
            }
            for (int i = 0; i < Settings.MAX_JOINTS; i++)
            {
                JointTransformsLocation[i] = GetUniformLocation("jointTransforms[" + i + "]");
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

        public void LoadSpecularMap(bool specularMap)
        {
            LoadBool(UseSpecularMapLocation, specularMap);
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
        public void LoadFakeLighting(bool fakeLighting)
        {
            LoadBool(UseFakeLightingLocation, fakeLighting);
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
        public void LoadJointTransforms(mat4[] matrices)
        {
            for (int i = 0; i < matrices.Length; i++)
            {
                LoadMatrix(JointTransformsLocation[i], matrices[i]);
            }
            LoadTransformCount(matrices.Length);
        }
        public void LoadTransformCount(int count)
        {
            LoadInt(JointTransformCountLocation, count);
        }
    }
}
