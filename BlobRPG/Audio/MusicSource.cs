using GlmSharp;
using NAudio.Vorbis;
using NAudio.Wave;
using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlobRPG.Audio
{
    public class MusicSource : AudioSource
    {
        private readonly IWaveProvider SampleProvider;
        private readonly VorbisWaveReader OriginalReader;
        private bool Readable { get; set; }
        private bool ManuallyStopped { get; set; }
        private bool Ended { get; set; }
        private readonly List<byte> ConvertedData;
        private byte[] ReadBuffer;
        private int Cursor;
        public new bool Loop
        {
            get
            {
                return _Loop;
            }
            set
            {
                _Loop = value;
            }
        }

        public int[] Buffers { get; private set; }
        private bool Paused { get; set; } = false;


        public MusicSource(IWaveProvider sampleProvider, VorbisWaveReader originalReader) : base()
        {
            SampleProvider = sampleProvider;
            ConvertedData = new();
            Buffers = new int[Settings.AudioBufferCount];
            Cursor = 0;
            Readable = true;
            OriginalReader = originalReader;

            Init();
        }

        public void Play()
        {
            ManuallyStopped = false;
            if (!Paused)
            {
                Cursor = 0;
                AL.SourceRewind(SourceId);
            }
            Paused = false;
            if (Ended)
            {
                Init(false);
                Ended = false;
            }
            AL.SourcePlay(SourceId);
            Task.Run(() =>
            {
                int state;
                do
                {
                    AL.GetSource(SourceId, ALGetSourcei.SourceState, out state);
                    UpdateStream();
                    Thread.Sleep(10);
                }
                while (state == (int)ALSourceState.Playing || Loop);

                if (!ManuallyStopped && state == (int)ALSourceState.Stopped)
                {
                    Ended = true;
                    Play();
                }

            });
        }
        public void Pause()
        {
            AL.SourcePause(SourceId);
        }
        public void Stop()
        {
            AL.SourceStop(SourceId);
            Cursor = 0;
            ManuallyStopped = true;
        }

        public override void CleanUp()
        {
            Stop();
            AL.DeleteSource(SourceId);
            AL.DeleteBuffers(Buffers);
        }
        private void Init(bool first = true)
        {
            if (first)
            {
                AL.GenBuffers(Buffers.Length, Buffers);

                Channels = SampleProvider.WaveFormat.Channels;
                SampleRate = SampleProvider.WaveFormat.SampleRate;

                ReadBuffer = new byte[Settings.AudioBufferSize];
            }
            int read;
            for (int i = 0; i < Buffers.Length; i++)
            {
                read = Read();
                StoreData(Buffers[i], read);
            }
            AL.SourceQueueBuffers(SourceId, Buffers);
            Paused = true;
        }
        private void UpdateStream()
        {
            AL.GetSource(SourceId, ALGetSourcei.BuffersProcessed, out int processed);
            if (processed <= 0) return;

            for (int i = 0; i < processed; i++)
            {
                int buff = AL.SourceUnqueueBuffer(SourceId);
                int read = Read();
                if (read > 0)
                {
                    StoreData(buff, read);
                    AL.SourceQueueBuffer(SourceId, buff);
                }
            }
        }

        private int Read()
        {
            if (Cursor + ReadBuffer.Length <= ConvertedData.Count)
            {
                int length = ReadFromCursor();
                ClearBuffer(length);
                return length;
            }
            if (!Readable)
            {
                int canTake = Math.Min(ConvertedData.Count - Cursor, ReadBuffer.Length);
                if (canTake != 0)
                    ReadFromCursor(length: canTake);
                if (!Loop)
                {
                    return canTake;
                }
                if (canTake != ReadBuffer.Length)
                {
                    int takeMore = ReadBuffer.Length - canTake;
                    Cursor = 0;
                    ReadFromCursor(canTake, takeMore);
                }
                return ReadBuffer.Length;
            }
            int read = SampleProvider.Read(ReadBuffer, 0, ReadBuffer.Length);

            if (read == ReadBuffer.Length)
            {
                ConvertedData.AddRange(ReadBuffer);
            }
            else
            {
                ConvertedData.AddRange(ReadBuffer.Take(read));
                Readable = false;
                OriginalReader.Close();
                OriginalReader.Dispose();
                ClearBuffer(read);
            }

            Cursor += read;
            return read;
        }
        private void StoreData(int buffer, int length = -1)
        {
            if (length == -1)
                AL.BufferData(buffer, Channels == 2 ? ALFormat.Stereo16 : ALFormat.Mono16, ReadBuffer, SampleRate);
            else
                AL.BufferData(buffer, Channels == 2 ? ALFormat.Stereo16 : ALFormat.Mono16, ref ReadBuffer[0], length, SampleRate);
        }

        private int ReadFromCursor(int offset = -1, int length = -1)
        {
            if (offset == -1) offset = 0;
            if (length == -1) length = Math.Min(ReadBuffer.Length, ConvertedData.Count - Cursor);

            for (int i = offset; i < length; i++)
            {
                ReadBuffer[i] = ConvertedData[Cursor + i];
            }
            Cursor += length;

            return length;
        }
        private void ClearBuffer(int offset = 0)
        {
            for (int i = offset; i < ReadBuffer.Length; i++)
            {
                ReadBuffer[i] = 0;
            }
        }
    }
}
