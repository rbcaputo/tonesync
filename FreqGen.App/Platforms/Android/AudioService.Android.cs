using Android.Media;
using Android.OS;
using FreqGen.Core;
using Microsoft.Extensions.Logging;

namespace FreqGen.App.Services
{
  /// <summary>
  /// Android-specific audio implementation using AudioTrack.
  /// Optimized for low-latency real-time audio playback.
  /// </summary>
  public sealed partial class AudioService
  {
    private AudioTrack? _audioTrack;
    private Thread? _audioThread;
    private volatile bool _isAudioThreadRunning;

    // Pre-allocated buffers (no allocations in audio loop)
    private readonly float[] _floatBuffer = new float[AudioSettings.RecommendedBufferSize];
    private readonly short[] _pcmBuffer = new short[AudioSettings.RecommendedBufferSize];

    partial void InitializePlatformAudio()
    {
      _logger.LogInformation("Initializing Android AudioTrack");

      try
      {
        // Query minimum buffer size
        int minBufferSize = AudioTrack.GetMinBufferSize(
          AudioSettings.SampleRate,
          ChannelOut.Mono,
          Encoding.Pcm16bit
        );

        if (minBufferSize == (int)TrackStatus.ErrorBadValue)
          throw new InvalidOperationException("Invalid audio format for this device");

        // Use larger buffer for stability (4x minimum)
        int bufferSizeInBytes = Math.Max(
          minBufferSize,
          AudioSettings.RecommendedBufferSize * sizeof(short) * 4
        );

        _logger.LogDebug(
          "AudioTrack buffer: min={MinSize}, using={ActualSize}",
          minBufferSize,
          bufferSizeInBytes
        );

        // Configure audio attributes
        AudioAttributes? audioAttributes = new AudioAttributes.Builder()?
          .SetUsage(AudioUsageKind.Media)?
          .SetContentType(AudioContentType.Music)?
          .Build();

        // Configure audio format
        AudioFormat? audioFormat = new AudioFormat.Builder()?
          .SetSampleRate(AudioSettings.SampleRate)?
          .SetEncoding(Encoding.Pcm16bit)?
          .SetChannelMask(ChannelOut.Mono)?
          .Build();

        if (audioAttributes is null || audioFormat is null)
          throw new InvalidOperationException("Failed to create audio configuration");

        // Build AudioTrack
        AudioTrack.Builder builder = new AudioTrack.Builder()
          .SetAudioAttributes(audioAttributes)
          .SetAudioFormat(audioFormat)
          .SetBufferSizeInBytes(bufferSizeInBytes)
          .SetTransferMode(AudioTrackMode.Stream);

        // Enable low-latency mode on Android 8.0+
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
          builder.SetPerformanceMode(AudioTrackPerformanceMode.LowLatency);
          _logger.LogDebug("Low-latency mode enabled");
        }

        _audioTrack = builder.Build();

        if (_audioTrack is null)
          throw new InvalidOperationException("Failed to create AudioTrack");

        _logger.LogInformation("Android AudioTrack initialized successfully");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Failed to initialize Android audio");
        throw new InvalidOperationException("Android audio initialization failed", ex);
      }
    }

    partial void StartPlatformAudio()
    {
      if (_isAudioThreadRunning || _audioTrack is null || _engine is null)
        return;

      _logger.LogInformation("Starting Android audio thread");

      try
      {
        // Start AudioTrack playback
        _audioTrack.Play();

        // Start audio rendering thread
        _isAudioThreadRunning = true;
        _audioThread = new(AudioThreadLoop)
        {
          Name = "FreqGen-Android-AudioThread",
          IsBackground = true,
          Priority = System.Threading.ThreadPriority.Highest
        };

        _audioThread.Start();
        _logger.LogInformation("Android audio thread started");
      }
      catch (Exception ex)
      {
        _isAudioThreadRunning = false;
        _logger.LogError(ex, "Failed to start Android audio");
        throw new InvalidOperationException("Failed to start Android audio", ex);
      }
    }

    partial void StopPlatformAudio()
    {
      if (!_isAudioThreadRunning)
        return;

      _logger.LogInformation("Stopping Android audio thread");

      try
      {
        // Signal thread to stop
        _isAudioThreadRunning = false;

        // Wait for thread to exit (with timeout)
        if (_audioThread is not null && _audioThread.IsAlive)
          if (!_audioThread.Join(TimeSpan.FromSeconds(2)))
            _logger.LogWarning("Audio thread did not stop gracefully");

        // Stop and flush AudioTrack
        _audioTrack?.Stop();
        _audioTrack?.Flush();

        _logger.LogInformation("Android audio thread stopped");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error stopping Android audio");
      }
    }

    partial void DisposePlatformAudio()
    {
      try
      {
        _audioTrack?.Release();
        _audioTrack?.Dispose();
        _audioTrack = null;
        _audioThread = null;

        _logger.LogInformation("Android audio resources disposed");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error disposing Android audio");
      }
    }

    /// <summary>
    /// Audio rendering loop running on dedicated high-priority thread.
    /// This is the HOT PATH - must be allocation-free and deterministic.
    /// </summary>
    private void AudioThreadLoop()
    {
      // Set real-time audio priority
      try
      {
        Process.SetThreadPriority(Android.OS.ThreadPriority.UrgentAudio);
        _logger.LogDebug("Audio thread priority set to URGENT_AUDIO");
      }
      catch (Exception ex)
      {
        _logger.LogWarning(ex, "Failed to set audio thread priority");
      }

      _logger.LogInformation("Audio thread loop started");

      while (_isAudioThreadRunning)
      {
        try
        {
          // Local copies for thread safety (avoid torn reads)
          AudioEngine? engine = _engine;
          AudioTrack? audioTrack = _audioTrack;

          if (engine is null || audioTrack is null || !_isAudioThreadRunning)
            break;

          // Fill buffer from audio engine (HOT PATH)
          engine.FillBuffer(_floatBuffer.AsSpan());

          // Convert float [-1.0, 1.0] to PCM16 [-32768, 32767]
          for (int i = 0; i < _floatBuffer.Length; i++)
          {
            float sample = _floatBuffer[i];

            // Clamp for safety (should already be clamped by engine)
            sample = Math.Clamp(sample, -1.0f, 1.0f);

            // Convert to PCM16
            _pcmBuffer[i] = (short)(sample * 32767f);
          }

          // Write to AudioTrack (blocking write)
          int bytesWritten = audioTrack.Write(
            _pcmBuffer,
            0,
            _pcmBuffer.Length,
            WriteMode.Blocking
          );

          if (bytesWritten < 0)
          {
            _logger.LogError("AudioTrack write error: {ErrorCode}", bytesWritten);
            break;
          }
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Audio thread loop error");
          break;
        }
      }

      _logger.LogInformation("Audio thread loop exited");
    }
  }
}
