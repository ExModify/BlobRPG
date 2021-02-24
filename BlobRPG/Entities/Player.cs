using BlobRPG.Input;
using BlobRPG.MainComponents;
using BlobRPG.Models;
using GlmSharp;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Entities
{
    public class Player : Entity
    {
        const float WalkSpeed = 10;
        const float RunSpeed = 20;

        const float JumpHeight = 8;

        private MainComponents.Window Window;

        public double CurrentVerticalSpeed { get; private set; }
        public double CurrentHorizontalSpeed { get; private set; }
        public double Accelerator { get; private set; }
        public double UpwardSpeed { get; private set; }

        public bool AllowFlight { get; set; } = false;

        public bool Render { get; set; } = false;
        bool InAir { get; set; } = false;


        public Player(TexturedModel model, vec3 position, MainComponents.Window window, float rx = 0, float ry = 0, float rz = 0, float scale = 1) : base(model, position, rx, ry, rz, scale)
        {
            Window = window;

            CurrentVerticalSpeed = 0;
            CurrentHorizontalSpeed = 0;
            Accelerator = 0;
            UpwardSpeed = 0;
        }

        public void Move(Terrain terrain)
        {
            ProcessInput();

            if (!InputManager.IsMouseRightDown && !InputManager.IsMouseLeftDown && Accelerator < 20)
            {
                float delta = InputManager.XDelta * 0.2f;
                if (InputManager.IsKeyDown(Keys.Space) && ((delta < 0 && InputManager.IsKeyDown(Keys.D)) || (delta > 0 && InputManager.IsKeyDown(Keys.A))))
                {
                    Accelerator += 0.01f;
                }
            }

            double d = CurrentVerticalSpeed * Accelerator * Window.DeltaTime;

            float dX = (float)(d * Math.Sin(MathHelper.DegreesToRadians(RotationY)));
            float dZ = (float)(d * Math.Cos(MathHelper.DegreesToRadians(RotationY)));

            d = CurrentHorizontalSpeed * Window.DeltaTime;

            dX += (float)(d * Math.Sin(MathHelper.DegreesToRadians(RotationY + 90)));
            dZ += (float)(d * Math.Cos(MathHelper.DegreesToRadians(RotationY + 90)));

            UpwardSpeed += Window.Gravity * Window.DeltaTime;
            float TerrainHeight = terrain.GetHeightOfTerrain(Position.x, Position.z);
            Position += new vec3(dX, (float)(UpwardSpeed * Window.DeltaTime), dZ);

            if (!(Position.x > terrain.Boundaries.x && Position.x < terrain.Boundaries.z))
            {
                Position -= new vec3(dX, 0, 0);
            }
            if (!(Position.z > terrain.Boundaries.y && Position.z < terrain.Boundaries.w))
            {
                Position -= new vec3(0, 0, dZ);
            }

            if (Position.y < TerrainHeight)
            {
                UpwardSpeed = 0;
                Position = new vec3(Position.x, TerrainHeight, Position.z);
                InAir = false;
            }

            if (Position.y < -Terrain.MaxHeight)
            {
                Position = new vec3(Position.x, TerrainHeight + 1, Position.z);
            }
        }
        private void Jump()
        {
            if (!InAir)
            {
                UpwardSpeed = JumpHeight;
                if (!AllowFlight)
                    InAir = true;
            }
        }

        private void ProcessInput()
        {
            if (InputManager.IsKeyDown(Keys.W))
            {
                if (InputManager.IsKeyDown(Keys.LeftShift) && !InAir)
                {
                    CurrentVerticalSpeed = RunSpeed;
                }
                else
                {
                    CurrentVerticalSpeed = WalkSpeed;
                }
            }
            else if (InputManager.IsKeyDown(Keys.S) && !InAir)
            {
                if (InputManager.IsKeyDown(Keys.LeftShift))
                {
                    CurrentVerticalSpeed = -RunSpeed;
                }
                else
                {
                    CurrentVerticalSpeed = -WalkSpeed;
                }
            }
            else if (!InputManager.IsKeyDown(Keys.Space))
            {
                CurrentVerticalSpeed = 0;
                Accelerator = 1;
            }
            if (InputManager.IsKeyDown(Keys.A))
            {
                if (InputManager.IsKeyDown(Keys.LeftShift))
                {
                    CurrentHorizontalSpeed = RunSpeed;
                }
                else
                {
                    CurrentHorizontalSpeed = WalkSpeed;
                }
            }
            else if (InputManager.IsKeyDown(Keys.D))
            {
                if (InputManager.IsKeyDown(Keys.LeftShift))
                {
                    CurrentHorizontalSpeed = -RunSpeed;
                }
                else
                {
                    CurrentHorizontalSpeed = -WalkSpeed;
                }
            }
            else
            {
                CurrentHorizontalSpeed = 0;
            }


            if (InputManager.IsKeyDown(Keys.Space))
            {
                Jump();
            }

            if (!InputManager.IsMouseLeftDown && !InputManager.IsMouseRightDown)
            {
                Rotate(0, -InputManager.XDelta * 0.2f, 0);
            }
        }
    }
}
