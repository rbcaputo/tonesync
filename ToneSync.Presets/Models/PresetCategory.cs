namespace ToneSync.Presets.Models
{
  /// <summary>
  /// Categorizes presets based on their therapeutic or mathematical approach.
  /// </summary>
  public enum PresetCategory
  {
    /// <summary>
    /// Amplitude modulation based on neural oscillation bands (0.5-100Hz).
    /// </summary>
    Brainwave,
    /// <summary>
    /// Ancient 6-tone scale frequencies (e.g., 528Hz).
    /// </summary>
    Solfeggio,
    /// <summary>
    /// Rhythmic pulsing using high-depth modulation or square waves.
    /// </summary>
    Isochronic,
    /// <summary>
    /// Stereo-field phase offsets for binaural entrainment.
    /// </summary>
    Binaural,
    /// <summary>
    /// User-created or experimental configurations.
    /// </summary>
    Custom
  }
}
