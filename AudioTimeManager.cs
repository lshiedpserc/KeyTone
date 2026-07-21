using Raylib_cs;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace KeyTone
{
    public static class AudioTimeManager
    {
        public static Sound CurrentTrack;
        public static bool IsLoaded = false;

        // Custom time tracking to maintain precision
        private static double _startTime = 0;
        private static bool _isPlaying = false;
        private static double _pauseOffset = 0;

        public static void Init()
        {
            if (!File.Exists("dummy_track.wav"))
            {
                GenerateDummyWav("dummy_track.wav", 44100, 120); // 120 seconds
            }

            CurrentTrack = Raylib.LoadSound("dummy_track.wav");
            IsLoaded = true;
        }

        public static void Play()
        {
            if (!IsLoaded) return;
            Raylib.PlaySound(CurrentTrack);
            _startTime = Raylib.GetTime();
            _isPlaying = true;
            _pauseOffset = 0;
        }

        public static void Pause()
        {
            if (!IsLoaded || !_isPlaying) return;
            Raylib.PauseSound(CurrentTrack);
            _isPlaying = false;
            _pauseOffset = Raylib.GetTime() - _startTime;
        }

        public static void Resume()
        {
            if (!IsLoaded || _isPlaying) return;
            Raylib.ResumeSound(CurrentTrack);
            _isPlaying = true;
            _startTime = Raylib.GetTime() - _pauseOffset;
        }

        public static void Stop()
        {
            if (!IsLoaded) return;
            Raylib.StopSound(CurrentTrack);
            _isPlaying = false;
            _pauseOffset = 0;
        }

        public static double GetCurrentTime()
        {
            if (!_isPlaying) return _pauseOffset;
            return Raylib.GetTime() - _startTime;
        }

        public static void Seek(double time)
        {
            if (!IsLoaded) return;
            // Raylib.SeekMusicStream or similar isn't available for Sound.
            // We might need to handle this carefully if using streams, but for now we manipulate time.
            _pauseOffset = Math.Max(0, time);
            if (_isPlaying)
            {
                _startTime = Raylib.GetTime() - _pauseOffset;
            }
        }

        public static void Unload()
        {
            if (IsLoaded)
            {
                Raylib.UnloadSound(CurrentTrack);
                IsLoaded = false;
            }
        }

        private static void GenerateDummyWav(string filename, int sampleRate, int durationSeconds)
        {
            int numSamples = sampleRate * durationSeconds;
            short[] samples = new short[numSamples];

            // Generate a simple click track (beep every second for 120BPM/60BPM sync)
            for (int i = 0; i < numSamples; i++)
            {
                if (i % sampleRate < sampleRate / 10) // 100ms beep every second
                {
                    double t = (double)i / sampleRate;
                    samples[i] = (short)(Math.Sin(2 * Math.PI * 440 * t) * 10000);
                }
                else
                {
                    samples[i] = 0;
                }
            }

            using (var writer = new BinaryWriter(File.Open(filename, FileMode.Create)))
            {
                writer.Write(new char[] { 'R', 'I', 'F', 'F' });
                writer.Write(36 + numSamples * 2);
                writer.Write(new char[] { 'W', 'A', 'V', 'E' });
                writer.Write(new char[] { 'f', 'm', 't', ' ' });
                writer.Write(16);
                writer.Write((short)1); // PCM
                writer.Write((short)1); // Mono
                writer.Write(sampleRate);
                writer.Write(sampleRate * 2);
                writer.Write((short)2);
                writer.Write((short)16); // bits per sample
                writer.Write(new char[] { 'd', 'a', 't', 'a' });
                writer.Write(numSamples * 2);

                byte[] bytes = new byte[numSamples * 2];
                Buffer.BlockCopy(samples, 0, bytes, 0, bytes.Length);
                writer.Write(bytes);
            }
        }
    }
}
