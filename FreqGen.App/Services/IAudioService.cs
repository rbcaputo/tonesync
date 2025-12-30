using FreqGen.Presets.Models;

namespace FreqGen.App.Services
{
  /// <summary>
  /// Platform-agnostic audio service interface.
  /// </summary>
  public interface IAudioService
  {
    /// <summary>
    /// Check if audio is currently playing.
    /// </summary>
    public bool IsPlaying { get; }

    /// <summary>
    /// Get the currently playing preset.
    /// </summary>
    FrequencyPreset? CurrentPreset { get; }

    /// <summary>
    /// Initialize the audio system.
    /// </summary>
    public Task InitializeAsync();

    /// <summary>
    /// Load and play a preset.
    /// </summary>
    public Task PlayPresetAsync(FrequencyPreset preset);

    /// <summary>
    /// Stop playback.
    /// </summary>
    Task StopAsync();

    /// <summary>
    /// Cleanup and release resources.
    /// </summary>
    Task DisposeAsync();
  }
}