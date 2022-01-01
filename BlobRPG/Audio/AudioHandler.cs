using BlobRPG.LoggerComponents;
using GlmSharp;
using NAudio.Vorbis;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlobRPG.Audio
{
    public class AudioHandler : ILogger
    {
        private static List<AudioSource> AudioSources { get; set; }

        private static new LogModule Module { get; set; } = LogModule.Audio;
        public static ALDevice Device { get; private set; }
        public static ALContext Context { get; private set; }

        public static bool Initialized { get; private set; } = false;

        public static void Init()
        {
            AudioSources = new();

            string device = null;
            if (!InitDevice(device))
            {
                Log(Error, "Couldn't initialize audio system on device: " + (device ?? "Default device"));
            }
            else
            {
                Initialized = true;
            }
        }
        public static void SetListenerData(vec3 position)
        {
            AL.Listener(ALListener3f.Position, position.x, position.y, position.z);
            AL.Listener(ALListener3f.Velocity, 0, 0, 0);
        }

        public static SfxSource LoadSfx(string file)
        {
            if (!Initialized) return null;
            FileStream fs = new(file, FileMode.Open);
            SfxSource sfx = LoadSfx(fs);
            fs.Close();
            return sfx;
        }
        public static SfxSource LoadSfx(Stream stream)
        {
            if (!Initialized) return null;

            int buff = AL.GenBuffer();

            using (VorbisWaveReader reader = new(stream, false))
            {
                StereoToMonoSampleProvider provider = new(reader);
                SampleToWaveProvider16 reader16 = new(provider);

                int channels = reader16.WaveFormat.Channels;
                int sampleRate = reader16.WaveFormat.SampleRate;

                var readBuffer = new byte[Settings.AudioBufferSize];
                List<byte> data = new();

                int read = 0;
                while ((read = reader16.Read(readBuffer, 0, readBuffer.Length)) > 0)
                {
                    if (read == readBuffer.Length)
                        data.AddRange(readBuffer);
                    else data.AddRange(readBuffer.Take(read));
                }
                
                AL.BufferData(buff, channels == 2 ? ALFormat.Stereo16 : ALFormat.Mono16, data.ToArray(), reader16.WaveFormat.SampleRate);
            }

            SfxSource src = new()
            {
                Buffer = buff
            };
            AudioSources.Add(src);
            return src;
        }

        public static MusicSource LoadMusic(string file)
        {
            if (!Initialized) return null;
            FileStream fs = new(file, FileMode.Open);
            MusicSource sfx = LoadMusic(fs);
            return sfx;
        }
        public static MusicSource LoadMusic(Stream stream)
        {
            if (!Initialized) return null;

            VorbisWaveReader reader = new(stream, false);
            SampleToWaveProvider16 reader16 = new(reader);

            MusicSource src = new(reader16, reader);
            AudioSources.Add(src);
            return src;
        }

        public static void CleanUp()
        {
            foreach (AudioSource b in AudioSources)
                b.CleanUp();
        }
        private static bool InitDevice(string devicename = null)
        {
            try
            {
                Device = ALC.OpenDevice(devicename);
                Context = ALC.CreateContext(Device, new ALContextAttributes());
                return ALC.MakeContextCurrent(Context);
            }
            catch
            {
                return false;
            }
        }

        private static new void Log(LogSeverity severity, string message)
        {
            Log(Module, severity, message);
        }
    }
}
