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
        private float LifeLength { get; set; }

        private float ElapsedTime { get; set; } = 0;

        public Particle(vec3 position, vec3 velocity, float gravity, float lifeLength, float rotation, float scale)
        {
            Position = position;
            Velocity = velocity;
            Gravity = gravity;
            LifeLength = lifeLength;
            Rotation = rotation;
            Scale = scale;
        }

        public bool Update()
        {

            return false;
        }
    }
}
