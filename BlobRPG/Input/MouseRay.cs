using BlobRPG.Entities;
using BlobRPG.MainComponents;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Input
{
    internal class MouseRay
    {
        public vec3 CurrentRay { get; private set; }

        private readonly mat4 InvertedProjectionMatrix;
        private readonly Camera Camera;
        private readonly Window Window;


        internal MouseRay(Camera camera, Window window, ref mat4 projectionMatrix)
        {
            Camera = camera;
            Window = window;

            InvertedProjectionMatrix = projectionMatrix.Inverse;
        }

        internal void Update()
        {
            float mouseX = InputManager.X;
            float mouseY = InputManager.Y;

            // normalized device coords
            float ndcX = (2f * mouseX) / Window.ClientSize.X - 1f;
            float ndcY = (2f * mouseY) / Window.ClientSize.Y - 1f;

            // clip coords
            vec4 clipCoords = new vec4(ndcX, ndcY, -1f, 1f);

            // eye coords
            clipCoords = InvertedProjectionMatrix * clipCoords;
            clipCoords = new vec4(clipCoords.x, clipCoords.y, -1, 0);

            // world coords
            vec4 rayWorld = Camera.ViewMatrix.Inverse * clipCoords;

            CurrentRay = rayWorld.Normalized.xyz;
        }

        
    }
}
