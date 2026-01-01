using FreqGen.Core;
using FreqGen.Presets.Models;

namespace FreqGen.Presets.Presets
{
  /// <summary>
  /// Manages the selection, loading, and state tracking of frequency presets.
  /// Acts as the primary interface between the UI and the AudioEngine.
  /// </summary>
  public sealed class PresetEngine(AudioEngine audioEngine)
  {
    private readonly AudioEngine _audioEngine = audioEngine;
    private readonly List<LayerConfiguration> _currentCoreConfigs = [];
    private FrequencyPreset? _activePreset;

    /// <summary>
    /// Gets the currently active preset (null if none).
    /// </summary>
    public FrequencyPreset? ActivePreset => _activePreset;

    /// <summary>
    /// Returns the current configurations as a read-only list for the audio thread.
    /// </summary>
    public IReadOnlyList<LayerConfiguration> GetCurrentConfigs() =>
      _currentCoreConfigs.AsReadOnly();

    /// <summary>
    /// Returns all existing presets.
    /// </summary>
    public static IEnumerable<FrequencyPreset> GetAllPresets() =>
      BrainwavePresets.All
        .Concat(SolfeggioPresets.All)
        .Concat(IsochronicPresets.All);

    /// <summary>
    /// Loads a preset and prepares the core configurations.
    /// Does not start playback automatically to allow for UI preparation.
    /// </summary>
    public void LoadPreset(FrequencyPreset preset)
    {
      preset.Validate();

      _activePreset = preset;

      SyncCoreConfigs();
    }

    /// <summary>
    /// Updates the core engine with the current preset configurations.
    /// This should be called before or during the FillBuffer cycle.
    /// </summary>
    public void SyncCoreConfigs()
    {
      if (_activePreset is null)
        return;

      _currentCoreConfigs.Clear();
      foreach (LayerConfig layer in _activePreset.Layers)
        _currentCoreConfigs.Add(layer.ToCoreConfig(_audioEngine.IsPlaying));
    }

    /// <summary>
    /// Resets the engine state to prevent clicks on restart.
    /// </summary>
    public void Reset()
    {
      _audioEngine.Reset();
      _currentCoreConfigs.Clear();
      _activePreset = null;
    }
  }
}
