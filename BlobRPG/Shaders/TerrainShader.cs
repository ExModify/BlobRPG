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
        private int LightPositionLocation;
        private int LightColorLocation;
        private int ReflectivityLocation;
        private int ShineDamperLocation;

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
            TransformationMatrixLocation = GetUniformLocation("transformationMatrix");
            ProjectionMatrixLocation = GetUniformLocation("projectionMatrix");
            ViewMatrixLocation = GetUniformLocation("viewMatrix");
            LightPositionLocation = GetUniformLocation("lightPosition");
            LightColorLocation = GetUniformLocation("lightColor");
            ReflectivityLocation = GetUniformLocation("reflectivity");
            ShineDamperLocation = GetUniformLocation("shineDamper");
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
        public void LoadLight(Light light)
        {
            LoadVector(LightPositionLocation, light.Position);
            LoadVector(LightColorLocation, light.Color);
        }

        public void LoadShineVariables(float reflectivity, float shineDamper)
        {
            LoadFloat(ReflectivityLocation, reflectivity);
            LoadFloat(ShineDamperLocation, shineDamper);
        }
    }
}
