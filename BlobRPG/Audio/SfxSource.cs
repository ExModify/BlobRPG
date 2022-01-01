using GlmSharp;
using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Audio
{
    public class SfxSource : AudioSource
    {
        private int _Buffer;

        public int Buffer
        {
            get
            {
                return _Buffer;
            }
            set
            {
                _Buffer = value;
                AL.Source(SourceId, ALSourcei.Buffer, value);
            }
        }

        public SfxSource() : base()
        {
        }

        public void Play()
        {
            Stop();
            AL.SourcePlay(SourceId);
        }
        public void Pause()
        {
            AL.SourcePause(SourceId);
        }
        public void Continue()
        {
            AL.SourcePlay(SourceId);
        }
        public void Stop()
        {
            AL.SourceStop(SourceId);
        }

        public override void CleanUp()
        {
            Stop();
            AL.DeleteSource(SourceId);
            AL.DeleteBuffer(Buffer);
        }


    }
}
