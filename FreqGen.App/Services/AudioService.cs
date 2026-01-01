using FreqGen.Core;
using FreqGen.Core.Exceptions;
using FreqGen.Presets.Models;
using FreqGen.Presets.Presets;
using Microsoft.Extensions.Logging;

namespace FreqGen.App.Services
{
  /// <summary>
  /// Cross-platform audio service implementation.
  /// </summary>
  public sealed partial class AudioService(ILogger<AudioService> logger) : IAudioService, IAsyncDisposable
  {
    private readonly ILogger<AudioService> _logger = logger;
    private AudioEngine? _engine;
    private PresetEngine? _presetEngine;
    private bool _isInitialized;
    private bool _isPlaying;
    private int _retryCount;
    private const int MaxRetries = 3;

    public bool IsPlaying => _isPlaying;
    public FrequencyPreset? CurrentPreset => _presetEngine?.CurrentPreset;

    public async Task InitializeAsync()
    {
      if (_isInitialized)
        return;

      try
      {
        _logger.LogInformation("Initializing audio service...");

        // Create core audio engine
        _engine = new(
          sampleRate: AudioSettings.SampleRate,
          layerCount: 6 // Support complex presets
        );

        // Create preset engine
        _presetEngine = new(_engine);

        // Initialize platform-specific audio
        InitializePlatformAudio();

        _isInitialized = true;
        _retryCount = 0;

        _logger.LogInformation("Audio service initialized successfully");
      }
      catch (AudioInitializationException ex)
      {
        _logger.LogError($"Audio initialization failed: {ex.Message}");
        throw;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Unexpected error during initialization: {ex}");
        throw new AudioInitializationException("Audio initialization failed", ex);
      }

      await Task.CompletedTask;
    }

    public async Task PlayPresetAsync(FrequencyPreset preset)
    {
      ArgumentNullException.ThrowIfNull(preset);

      if (!_isInitialized)
      {
        _logger.LogInformation("Audio not initialized, initializing now...");
        await InitializeAsync();
      }

      try
      {
        _logger.LogInformation($"Playing preset: {preset.DisplayName}");

        // Validate preset before loading
        preset.Validate();

        // Stop current playback
        if (_isPlaying)
        {
          _logger.LogDebug("Stopping current playback");
          await StopAsync();
        }

        // Load and play preset
        _presetEngine?.LoadAndPlay(preset);

        // Start platform audio
        StartPlatformAudio();

        _isPlaying = true;
        _retryCount = 0;

        _logger.LogInformation($"Successfully started preset: {preset.DisplayName}");
      }
      catch (PresetValidationException ex)
      {
        _logger.LogError($"Preset validation failed: {ex.Message}");
        throw;
      }
      catch (AudioPlaybackException ex)
      {
        _logger.LogError($"Playback failed: {ex.Message}");

        // Attempt retry if we haven't exceeded max retries
        if (_retryCount < MaxRetries)
        {
          _retryCount++;
          _logger.LogWarning($"Retrying playback (attempt {_retryCount}/{MaxRetries})...");
          await Task.Delay(500); // Brief delay before retry
          await PlayPresetAsync(preset);
          return;
        }

        throw;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Unexpected error during playback: {ex}");
        throw new AudioPlaybackException("Failed to start playback", ex);
      }
    }

    public async Task StopAsync()
    {
      if (!_isPlaying)
        return;

      try
      {
        _logger.LogInformation("Stopping audio playback");

        // Stop preset engine (begins fade-out)
        _presetEngine?.Stop();

        // Stop platform audio
        StopPlatformAudio();

        _isPlaying = false;

        _logger.LogInformation("Audio playback stopped successfully");
      }
      catch (Exception ex)
      {
        _logger.LogError($"Error while stopping: {ex}");
        throw new AudioPlaybackException("Failed to stop playback", ex);
      }

      await Task.CompletedTask;
    }

    public async Task<bool> RetryInitializationAsync()
    {
      _logger.LogInformation("Retrying audio initialization...");

      try
      {
        // Dispose existing resources
        await DisposeAsync();

        // Wait a moment
        await Task.Delay(1000);

        // Try again
        await InitializeAsync();
        return true;
      }
      catch (Exception ex)
      {
        _logger.LogError($"Retry failed: {ex}");
        return false;
      }
    }

    public async ValueTask DisposeAsync()
    {
      if (_isPlaying)
        await StopAsync();

      DisposePlatformAudio();

      _engine?.Dispose();
      _engine = null;
      _presetEngine = null;
      _isInitialized = false;

      _logger.LogInformation("Audio service disposed");
    }

    // Platform-specific methods (implemented in partial classes)
    partial void InitializePlatformAudio();
    partial void StartPlatformAudio();
    partial void StopPlatformAudio();
    partial void DisposePlatformAudio();
  }
}
