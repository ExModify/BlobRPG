﻿using BlobRPG.Input;
using BlobRPG.MainComponents;
using BlobRPG.Models;
using BlobRPG.Tools;
using GlmSharp;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlobRPG.Entities
{
    public class Camera
    {
        private float DistanceFromPlayer = 30;
        private float AngleAroundPlayer = 0;

        private vec3 position;
        private float WaterTileDistance { get; set; }

        internal vec3 Position { get => position; private set => position = value; }
        internal float Pitch { get; private set; }
        internal float Yaw { get; private set; }
        internal float Roll { get; private set; }

        internal mat4 ViewMatrix { get; private set; }

        private readonly Player Player;

        public Camera(Player player, vec3 position = new vec3(), float pitch = 20, float yaw = 0, float roll = 0)
        {
            Player = player;

            Position = position;
            Pitch = pitch;
            Yaw = yaw;
            Roll = roll;

            UpdateViewMatrix();
        }

        public void MoveUnderWaterTile(WaterTile tile)
        {
            WaterTileDistance = 2 * (position.y - tile.Height);
            position.y -= WaterTileDistance;
            Pitch = -Pitch;
            UpdateViewMatrix();
        }
        public void RevertWaterTileMove()
        {
            position.y += WaterTileDistance;
            Pitch = -Pitch;
            UpdateViewMatrix();
        }
        internal void Move()
        {
            CalculateZoom();
            CalculatePitch();
            CalculateAngleAroundPlayer();

            float horizontalDistance = (float)(DistanceFromPlayer * Math.Cos(MathHelper.DegreesToRadians(Pitch)));
            float verticalDistance = (float)(DistanceFromPlayer * Math.Sin(MathHelper.DegreesToRadians(Pitch)));

            if (DistanceFromPlayer == 0)
            {
                verticalDistance = 2;
            }

            float alpha = MathHelper.DegreesToRadians(Player.RotationY + AngleAroundPlayer);
            float offsetX = (float)(horizontalDistance * Math.Sin(alpha));
            float offsetZ = (float)(horizontalDistance * Math.Cos(alpha));

            Position = new vec3(Player.Position.x - offsetX,
                                Player.Position.y + verticalDistance,
                                Player.Position.z - offsetZ);

            Yaw = 180 - (Player.RotationY + AngleAroundPlayer);

            UpdateViewMatrix();
        }

        private void CalculateZoom()
        {
            float zoomLevel = InputManager.ScrollDelta * 5f;
            DistanceFromPlayer -= zoomLevel;
            DistanceFromPlayer = Math.Clamp(DistanceFromPlayer, 0, Settings.MaxZoom);

            if (DistanceFromPlayer == 0)
            {
                Player.Render = false;
            }
            else
            {
                Player.Render = true;
            }
        }
        private void CalculatePitch()
        {
            float pitchChange = (float)(InputManager.YDelta * Settings.DeltaTime * 40);
            Pitch += pitchChange;
            Pitch = MathHelper.Clamp(Pitch, 5, 60);
            Console.WriteLine(Pitch);
        }
        private void CalculateAngleAroundPlayer()
        {
            if (InputManager.IsMouseRightDown && InputManager.IsMouseLeftDown)
            {
                float angleChange = InputManager.XDelta * 0.2f;
                AngleAroundPlayer -= angleChange;
            }
        }

        private void UpdateViewMatrix()
        {
            ViewMatrix = MatrixMaths.CreateViewMatrix(this);
        }
    }
}
