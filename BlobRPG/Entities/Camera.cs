using BlobRPG.Input;
using BlobRPG.Tools;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Entities
{
    public class Camera
    {
        private vec3 position;

        public vec3 Position { get => position; private set => position = value; }
        public float Pitch { get; private set; }
        public float Yaw { get; private set; }
        public float Roll { get; private set; }

        public mat4 ViewMatrix { get; private set; }

        public Camera(vec3 position, float pitch, float yaw, float roll)
        {
            Position = position;
            Pitch = pitch;
            Yaw = yaw;
            Roll = roll;

            UpdateViewMatrix();
        }

        public void Move()
        {
            if (InputManager.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.W))
            {
                position.z -= 0.02f;
            }
            if (InputManager.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.S))
            {
                position.z += 0.02f;
            }
            if (InputManager.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.D))
            {
                position.x += 0.02f;
            }
            if (InputManager.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.A))
            {
                position.x -= 0.02f;
            }

            UpdateViewMatrix();
        }

        private void UpdateViewMatrix()
        {
            ViewMatrix = MatrixMaths.CreateViewMatrix(this);
        }
    }
}
