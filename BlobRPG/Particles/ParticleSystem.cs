using BlobRPG.Entities;
using BlobRPG.Textures;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Particles
{
    public class ParticleSystem
    {
        public float ParticlePerSecond { get; set; }
        public float Speed { get; set; }
        public float Gravity { get; set; }
        public float Life { get; set; }
        public float Scale { get; set; }

        private float _SpeedError;
        public float SpeedError
        {
            get
            {
                return _SpeedError;
            }
            set
            {
                _SpeedError = value * Speed;
            }
        }
        private float _LifeError;
        public float LifeError
        {
            get
            {
                return _LifeError;
            }
            set
            {
                _LifeError = value * Life;
            }
        }
        private float _ScaleError;
        public float ScaleError
        {
            get
            {
                return _ScaleError;
            }
            set
            {
                _ScaleError = value * Scale;
            }
        }
        private float _Deviation = 0;
        public float Deviation
        {
            get
            {
                return _Deviation;
            }
            set
            {
                _Deviation = (float)(value * Math.PI);
            }
        }

        public vec3? Direction { get; set; }
        public bool RandomRotation { get; set; }

        public ParticleTexture Texture { get; set; }


        private readonly Random Rng;

        public ParticleSystem(ParticleTexture texture, float particlePerSecond, float speed, float gravity, float life, float scale)
        {
            Texture = texture;
            ParticlePerSecond = particlePerSecond;
            Speed = speed;
            Gravity = gravity;
            Life = life;
            Scale = scale;

            Rng = new Random();
        }

        public void GenerateParticles(vec3 systemCenter)
        {
            float particlesToCreate = (float)(ParticlePerSecond * ((float)Settings.DeltaTime * 1000));
            int count = (int)Math.Floor(particlesToCreate);
            float partialParticle = particlesToCreate % 1;

            for (int i = 0; i < count; i++)
            {
                EmitParticle(systemCenter);
            }
            if (Rng.NextDouble() < partialParticle)
            {
                EmitParticle(systemCenter);
            }
        }

        private void EmitParticle(vec3 center)
        {
            vec3 velocity;
            if (Direction != null)
            {
                velocity = GenerateRandomUnitVectorWithinCone(Direction.Value, Deviation).Normalized;
            }
            else
            {
                velocity = GenerateRandomUnitVector().Normalized;
            }
            float val = GenerateValue(Speed, SpeedError);
            velocity *= val;
            float scale = GenerateValue(Scale, ScaleError);
            float lifeLength = GenerateValue(Life, LifeError);
            new Particle(Texture, new vec3(center), velocity, Gravity, lifeLength, GenerateRotation(), scale);
        }

        private float GenerateValue(float average, float errorMargin)
        {
            float offset = ((float)Rng.NextDouble() - 0.5f) * 2f * errorMargin;
            return average + offset;
        }

        private float GenerateRotation()
        {
            if (RandomRotation)
            {
                return (float)(Rng.NextDouble() * 360f);
            }
            else
            {
                return 0;
            }
        }

        private vec3 GenerateRandomUnitVectorWithinCone(vec3 coneDirection, float angle)
        {
            float cosAngle = (float)Math.Cos(angle);
            float theta = (float)(Rng.NextDouble() * 2f * Math.PI);
            float z = cosAngle + (float)(Rng.NextDouble() * (1 - cosAngle));
            float rootOneMinusZSquared = (float)Math.Sqrt(1 - z * z);
            float x = (float)(rootOneMinusZSquared * Math.Cos(theta));
            float y = (float)(rootOneMinusZSquared * Math.Sin(theta));

            vec4 direction = new(x, y, z, 1);
            if (coneDirection.x != 0 || coneDirection.y != 0 || (coneDirection.z != 1 && coneDirection.z != -1))
            {
                vec3 rotateAxis = vec3.Cross(coneDirection, new vec3(0, 0, 1)).Normalized;
                float rotateAngle = (float)Math.Acos(vec3.Dot(coneDirection, new vec3(0, 0, 1)));
                mat4 rotationMatrix = mat4.Rotate(-rotateAngle, rotateAxis);


                direction = rotationMatrix * direction;

            }
            else if (coneDirection.z == -1)
            {
                direction.z *= -1;
            }
            return new vec3(direction);
        }

        private vec3 GenerateRandomUnitVector()
        {
            float theta = (float)(Rng.NextDouble() * 2f * Math.PI);
            float z = (float)(Rng.NextDouble() * 2) - 1;
            float rootOneMinusZSquared = (float)Math.Sqrt(1 - z * z);
            float x = (float)(rootOneMinusZSquared * Math.Cos(theta));
            float y = (float)(rootOneMinusZSquared * Math.Sin(theta));
            return new vec3(x, y, z);
        }
    }
}
