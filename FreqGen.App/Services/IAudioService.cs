using FreqGen.Presets.Models;

namespace FreqGen.App.Services
{
  /// <summary>
  /// Platform-agnostic audio service interface.
  /// </summary>
  public interface IAudioService : IAsyncDisposable
  {
    /// <summary>
    /// Check if audio is currently playing.
    /// </summary>
    bool IsPlaying { get; }

    /// <summary>
    /// Get the currently playing preset.
    /// </summary>
    FrequencyPreset? CurrentPreset { get; }

    /// <summary>
    /// Initialize the audio system.
    /// </summary>
    Task InitializeAsync();

    /// <summary>
    /// Load and play a preset.
    /// </summary>
    Task PlayPresetAsync(FrequencyPreset preset);

    /// <summary>
    /// Stop playback.
    /// </summary>
    Task StopAsync();

    /// <summary>
    /// Retry initialization after a failure.
    /// </summary>
    /// <returns>True if retry succeeded, false otherwise</returns>
    Task<bool> RetryInitializationAsync();
  }
}
