using GlmSharp;
using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Audio
{
    public abstract class AudioSource
    {
        public int SourceId { get; private set; }

        public int Channels { get; protected set; }
        public int SampleRate { get; protected set; }

        protected float _Pitch;
        protected float _Volume;
        protected bool _Loop;
        protected vec3 _Position;
        protected vec3 _Velocity;

        public float Pitch
        {
            get
            {
                return _Pitch;
            }
            set
            {
                _Pitch = value;
                AL.Source(SourceId, ALSourcef.Pitch, value);
            }
        }
        public float Volume
        {
            get
            {
                return _Volume;
            }
            set
            {
                _Volume = value;
                AL.Source(SourceId, ALSourcef.Gain, value);
            }
        }
        public vec3 Position
        {
            get
            {
                return _Position;
            }
            set
            {
                _Position = value;
                AL.Source(SourceId, ALSource3f.Position, _Position.x, _Position.y, _Position.z);
            }
        }
        public vec3 Velocity
        {
            get
            {
                return _Velocity;
            }
            set
            {
                _Velocity = value;
                AL.Source(SourceId, ALSource3f.Velocity, _Velocity.x, _Velocity.y, _Velocity.z);
            }
        }
        public bool Loop
        {
            get
            {
                return _Loop;
            }
            set
            {
                _Loop = value;
                AL.Source(SourceId, ALSourceb.Looping, value);
            }
        }
        public bool Playing
        {
            get
            {
                AL.GetSource(SourceId, ALGetSourcei.SourceState, out int playing);
                return playing == (int)ALSourceState.Playing;
            }
        }

        public AudioSource()
        {
            SourceId = AL.GenSource();
        }

        public abstract void CleanUp();
    }
}
