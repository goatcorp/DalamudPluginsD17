using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;

namespace AetherFM;

public class RadioManager : IDisposable
{
    private IWavePlayer? waveOut;
    private MediaFoundationReader? reader;
    private Thread? radioThread;
    private CancellationTokenSource? cts;
    private string status = "Stopped";
    private string? currentUrl;
    private Action<string>? statusCallback;

    public bool Start(string url, Action<string>? onStatusChanged = null, float volume = 0.5f)
    {
        if (waveOut != null || radioThread != null)
        {
            Stop();
        }
        if (string.IsNullOrWhiteSpace(url))
        {
            status = "Error: empty URL";
            onStatusChanged?.Invoke(status);
            return false;
        }
        statusCallback = onStatusChanged;
        cts = new CancellationTokenSource();
        radioThread = new Thread(() => PlayRadio(url, cts.Token, volume));
        radioThread.IsBackground = true;
        radioThread.Start();
        return true;
    }

    private void PlayRadio(string url, CancellationToken token, float volume)
    {
        try
        {
            status = "Connecting...";
            statusCallback?.Invoke(status);
            currentUrl = url;
            using (reader = new MediaFoundationReader(url))
            using (waveOut = new WaveOutEvent())
            {
                waveOut.Init(reader);
                try { waveOut.Volume = volume; } catch { /* Not supported on all APIs, ignore if fails */ }
                waveOut.Play();
                status = "Playing";
                statusCallback?.Invoke(status);
                while (waveOut.PlaybackState == PlaybackState.Playing && !token.IsCancellationRequested)
                {
                    Thread.Sleep(200);
                }
                waveOut.Stop();
            }
            status = "Stopped";
            statusCallback?.Invoke(status);
        }
        catch (Exception ex)
        {
            status = $"Error: {ex.Message}";
            statusCallback?.Invoke(status);
        }
        finally
        {
            waveOut?.Dispose();
            reader?.Dispose();
            waveOut = null;
            reader = null;
            radioThread = null;
            cts = null;
        }
    }

    public void Stop()
    {
        cts?.Cancel();
        waveOut?.Stop();
        status = "Stopped";
        statusCallback?.Invoke(status);
    }

    public string GetStatus() => status;

    public void Dispose()
    {
        Stop();
    }

    public void SetVolume(float volume)
    {
        if (waveOut != null)
        {
            try { waveOut.Volume = volume; } catch { /* Ignora se non supportato */ }
        }
    }
} 