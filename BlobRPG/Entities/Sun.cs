using BlobRPG.MainComponents;
using BlobRPG.Textures;
using GlmSharp;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Entities
{
    public class Sun : Light
    {
        public ModelTexture CurrentTexture { get; set; }
        public float DistanceFromPlayer { get; set; } = 400f;
        public Player Player { get; private set; }
        public bool IsNight
        {
            get
            {
                return Settings.IngameTime > 12000;
            }
        }

        public Sun(vec3 position, vec3 color, Player player, vec3 attenuation = default) : base(position, color, attenuation)
        {
            Player = player;
        }

        public void Update()
        {
            float angle;

            if (Settings.IngameTime >= Settings.DayPhaseStart && Settings.IngameTime < Settings.DayPhaseEnd)
            {
                float progression = (Settings.IngameTime - Settings.DayPhaseStart) / Settings.PhaseTime;
                Color = Lerp3(Settings.NightLight, Settings.NoLight, Settings.DayLight, progression, 0.05f);

                angle = (190 + (20 * ((progression - 0.5f) * 2.0f))) % 190;
                Settings.SkyColor = vec3.Lerp(Settings.NightColor, Settings.DayColor, progression);
            }
            else if (Settings.IngameTime >= Settings.DayPhaseEnd && Settings.IngameTime < Settings.NightPhaseStart)
            {
                Color = Settings.DayLight;
                angle = 20 + (150 * ((Settings.IngameTime - Settings.DayPhaseEnd) / (Settings.DayDuration)));
                Settings.SkyColor = Settings.DayColor;
            }
            else if (Settings.IngameTime >= Settings.NightPhaseStart && Settings.IngameTime < Settings.NightPhaseEnd)
            {
                float progression = (Settings.IngameTime - Settings.NightPhaseStart) / Settings.PhaseTime;
                Color = Lerp3(Settings.DayLight, Settings.NoLight, Settings.NightLight, progression, 0.05f);

                angle = (190 + (20 * ((progression - 0.5f) * 2.0f))) % 190;

                Settings.SkyColor = vec3.Lerp(Settings.DayColor, Settings.NightColor, progression);
            }
            else
            {
                Color = Settings.NightLight;
                float nightProgression;
                if (Settings.IngameTime > Settings.NightPhaseEnd)
                {
                    nightProgression = Settings.IngameTime - Settings.NightPhaseEnd;
                }
                else
                {
                    nightProgression = Settings.MaxTime - Settings.NightPhaseEnd + Settings.IngameTime;
                }
                angle = 20 + (150 * (nightProgression / Settings.NightDuration));
                Settings.SkyColor = Settings.NightColor;
            }

            float horizontalDistance = (float)(DistanceFromPlayer * Math.Cos(MathHelper.DegreesToRadians(angle)));
            float verticalDistance = (float)(DistanceFromPlayer * Math.Sin(MathHelper.DegreesToRadians(angle)));

            float offsetX = (float)(horizontalDistance * Math.Sin(0));
            float offsetZ = (float)(horizontalDistance * Math.Cos(0));

            Position = new vec3(Player.Position.x - offsetX,
                                Player.Position.y + verticalDistance,
                                Player.Position.z - offsetZ);
        }

        private static vec3 Lerp3(vec3 a, vec3 b, vec3 c, float progression, float offset = 0.1f)
        {
            if (progression <= (0.5f - offset))
            {
                return vec3.Lerp(a, b, progression * (1 / (0.5f - offset)));
            }
            else if (progression >= (0.5f + offset))
            {
                return vec3.Lerp(b, c, (progression - 0.5f - offset) * (1 / (0.5f - offset)));
            }
            else
            {
                return b;
            }
        }
    }
}
