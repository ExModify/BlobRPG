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
        public double CurrentVerticalSpeed { get; private set; }
        public double CurrentHorizontalSpeed { get; private set; }
        public double Accelerator { get; private set; }
        public double UpwardSpeed { get; private set; }

        public bool Render { get; set; } = false;
        bool InAir { get; set; } = false;


        public Player(TexturedModel model, vec3 position, float rx = 0, float ry = 0, float rz = 0, float scale = 1, int textureIndex = 1) : base(model, position, rx, ry, rz, scale, textureIndex)
        {
            CurrentVerticalSpeed = 0;
            CurrentHorizontalSpeed = 0;
            Accelerator = 1;
            UpwardSpeed = 0;
        }

        public void Move(List<Terrain> terrains)
        {
            ProcessInput();

            double d = CurrentVerticalSpeed * Accelerator * Settings.DeltaTime;

            float dX = (float)(d * Math.Sin(MathHelper.DegreesToRadians(RotationY)));
            float dZ = (float)(d * Math.Cos(MathHelper.DegreesToRadians(RotationY)));

            d = CurrentHorizontalSpeed * Accelerator * Settings.DeltaTime;

            dX += (float)(d * Math.Sin(MathHelper.DegreesToRadians(RotationY + 90)));
            dZ += (float)(d * Math.Cos(MathHelper.DegreesToRadians(RotationY + 90)));

            UpwardSpeed += Settings.Gravity * Settings.DeltaTime;


            float dY = (float)(UpwardSpeed * Settings.DeltaTime);

            float terrainHeight = 0;
            for (int i = 0; i < terrains.Count; i++)
            {
                if (terrains[i].OnTerrain(Position.x, Position.z))
                {
                    terrainHeight = terrains[i].GetHeightOfTerrain(Position.x, Position.z);
                    break;
                }
            }

            if (Position.y + dY < terrainHeight)
            {
                UpwardSpeed = 0;
                dY = terrainHeight - Position.y;
                InAir = false;
            }

            Position = new vec3(Position.x + dX, Position.y + dY, Position.z + dZ);
        }
        private void Jump()
        {
            if (!InAir)
            {
                UpwardSpeed = Settings.JumpHeight;
                if (!Settings.AllowFlight)
                    InAir = true;
            }
        }

        private void ProcessInput()
        {
            ProcessMovementInput();


            if (InputManager.IsKeyDown(Keys.Space))
            {
                Jump();
            }

            if (!InputManager.IsMouseLeftDown && !InputManager.IsMouseRightDown)
            {
                Rotate(0, -InputManager.XDelta * 0.2f, 0);
            }
        }


        private void ProcessMovementInput()
        {
            if (InputManager.IsKeyDown(Keys.W))
            {
                if (InputManager.IsKeyDown(Keys.LeftShift))
                {
                    CurrentVerticalSpeed = Settings.RunSpeed;
                }
                else
                {
                    CurrentVerticalSpeed = Settings.WalkSpeed;
                }
            }
            else if (InputManager.IsKeyDown(Keys.S))
            {
                if (InputManager.IsKeyDown(Keys.LeftShift))
                {
                    CurrentVerticalSpeed = -Settings.RunSpeed;
                }
                else
                {
                    CurrentVerticalSpeed = -Settings.WalkSpeed;
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
                    CurrentHorizontalSpeed = Settings.RunSpeed;
                }
                else
                {
                    CurrentHorizontalSpeed = Settings.WalkSpeed;
                }
            }
            else if (InputManager.IsKeyDown(Keys.D))
            {
                if (InputManager.IsKeyDown(Keys.LeftShift))
                {
                    CurrentHorizontalSpeed = -Settings.RunSpeed;
                }
                else
                {
                    CurrentHorizontalSpeed = -Settings.WalkSpeed;
                }
            }
            else
            {
                CurrentHorizontalSpeed = 0;
            }

        }
    }
}
