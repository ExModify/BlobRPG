using BlobRPG.Particles;
using BlobRPG.Textures;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Entities
{
    public class Particle
    {
        public vec3 Position { get; private set; }
        public float Rotation { get; private set; }
        public float Scale { get; private set; }

        private vec3 Velocity { get; set; }
        private float Gravity { get; set; }
        private double LifeLength { get; set; }

        private double ElapsedTime { get; set; } = 0;

        public vec2 TextureOffset1 { get; set; }
        public vec2 TextureOffset2 { get; set; }
        public float BlendFactor { get; set; }

        public ParticleTexture Texture { get; set; }
        public float Distance { get; private set; }


        public Particle(ParticleTexture texture, vec3 position, vec3 velocity, float gravity, double lifeLength, float rotation, float scale)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            Gravity = gravity;
            LifeLength = lifeLength;
            Rotation = rotation;
            Scale = scale;

            ParticleHandler.Add(this);
        }

        public bool Update(Camera camera)
        {
            Velocity = new vec3(Velocity.x, (float)(Velocity.y + (Settings.Gravity * Gravity * Settings.DeltaTime * 1000)), Velocity.z);
            vec3 change = Velocity * ((float)Settings.DeltaTime * 1000);

            Position += change;
            Distance = (camera.Position - Position).LengthSqr;

            ElapsedTime += (Settings.DeltaTime * 1000);

            UpdateTexture();

            return ElapsedTime < LifeLength;
        }

        private void UpdateTexture()
        {
            double lifeFactor = ElapsedTime / LifeLength;
            int stageCount = Texture.NumberOfRows * Texture.NumberOfRows;

            double atlasProgression = lifeFactor * stageCount;

            int index1 = (int)atlasProgression;
            int index2 = index1 < stageCount - 1 ? index1 + 1 : index1;

            BlendFactor = (float)(atlasProgression % 1);

            TextureOffset1 = SetTextureOffset(index1);
            TextureOffset2 = SetTextureOffset(index2);
        }

        private vec2 SetTextureOffset(int Index)
        {
            int column = Index % Texture.NumberOfRows;
            int row = Index / Texture.NumberOfRows;
            return new vec2((float)column / Texture.NumberOfRows, (float)row / Texture.NumberOfRows);
        }
    }
}
