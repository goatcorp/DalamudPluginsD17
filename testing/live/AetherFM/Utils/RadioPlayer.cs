using NAudio.Wave;
using System;

namespace AetherFM.Utils
{
    public class RadioPlayer : IDisposable
    {
        private IWavePlayer waveOut;
        private MediaFoundationReader reader;

        public void Play(string url)
        {
            Stop();
            try
            {
                reader = new MediaFoundationReader(url);
                waveOut = new WaveOutEvent();
                waveOut.Init(reader);
                waveOut.Play();
            }
            catch (Exception ex)
            {
                Stop();
                throw new Exception($"Error during stream playback: {ex.Message}");
            }
        }

        public void Stop()
        {
            waveOut?.Stop();
            reader?.Dispose();
            waveOut?.Dispose();
            reader = null;
            waveOut = null;
        }

        public void Dispose()
        {
            Stop();
        }
    }
} 