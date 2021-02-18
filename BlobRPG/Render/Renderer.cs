using BlobRPG.Entities;
using BlobRPG.Models;
using BlobRPG.Shaders;
using GlmSharp;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlobRPG.Render
{
    public class Renderer
    {
        private const float FOV = 70;
        private const float NEAR = 0.1f;
        private const float FAR = 1000f;

        mat4 ProjectionMatrix;

        readonly EntityRenderer EntityRenderer;
        readonly EntityShader EntityShader;

        readonly Dictionary<TexturedModel, List<Entity>> Entities;

        public Renderer(GameWindow window)
        {
            Entities = new Dictionary<TexturedModel, List<Entity>>();
            window.Resize += (s) =>
            {
                CreateProjectionMatrix(FOV, s.Width, s.Height, NEAR, FAR);
                UpdateProjectionMatrix();
            };


            CreateProjectionMatrix(FOV, window.ClientSize.X, window.ClientSize.Y, NEAR, FAR);

            EntityShader = new EntityShader();
            EntityRenderer = new EntityRenderer(EntityShader, ref ProjectionMatrix);

            UpdateProjectionMatrix();
        }
        
        public void Render()
        {
            Prepare();

            EntityRenderer.Render(Entities);

            Entities.Clear();
        }

        

        public void ProcessObject(Entity entity)
        {
            if (Entities.Keys.Contains(entity.Model))
            {
                Entities[entity.Model].Add(entity);
            }
            else
            {
                Entities.Add(entity.Model, new List<Entity>() { entity });
            }
        }

        private void Prepare()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(1, 0, 0, 1);
        }

        private void CreateProjectionMatrix(float fov, int width, int height, float near, float far)
        {
            ProjectionMatrix = mat4.PerspectiveFov(fov, width, height, near, far);
        }

        private void UpdateProjectionMatrix()
        {
            EntityShader.Start();
            EntityShader.LoadProjectionMatrix(ProjectionMatrix);
            EntityShader.Stop();
        }
    }
}
