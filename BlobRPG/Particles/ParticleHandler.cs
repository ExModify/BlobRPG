using BlobRPG.Entities;
using BlobRPG.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Particles
{
    class ParticleHandler
    {
        public static Dictionary<ParticleTexture, List<Particle>> Particles = new();

        public static void Update(Camera camera)
        {
            for (int i = 0; i < Particles.Keys.Count; i++)
            {
                ParticleTexture key = Particles.Keys.ElementAt(i);
                for (int j = 0; j < Particles[key].Count; j++)
                {
                    bool alive = Particles[key][j].Update(camera);
                    if (!alive)
                    {
                        Particles[key].RemoveAt(j);
                        j--;
                    }
                }
                if (Particles[key].Count == 0)
                {
                    Particles.Remove(key);
                    i--;
                }
                else
                {
                    if (!key.Additive)
                        SortParticles(Particles[key]);
                }
            }

        }


        public static void Add(Particle particle)
        {
            if (Particles.ContainsKey(particle.Texture))
            {
                Particles[particle.Texture].Add(particle);
            }
            else
            {
                Particles.Add(particle.Texture, new List<Particle>() { particle });
            }
        }


        public static void SortParticles(List<Particle> particles)
        {
            for (int i = 1; i < particles.Count; i++)
            {
                Particle item = particles[i];
                if (item.Distance > particles[i - 1].Distance)
                {
                    SortUpHighToLow(ref particles, i);
                }
            }
        }
        private static void SortUpHighToLow(ref List<Particle> list, int i)
        {
            Particle item = list[i];
            int attemptPos = i - 1;
            while (attemptPos != 0 && list[attemptPos - 1].Distance < item.Distance)
            {
                attemptPos--;
            }
            list.RemoveAt(i);
            list.Insert(attemptPos, item);
        }
    }
}
