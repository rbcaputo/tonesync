using FreqGen.Core;
using FreqGen.Core.Exceptions;
using FreqGen.Presets.Exceptions;

namespace FreqGen.Presets.Models
{
  /// <summary>
  /// Defines a complete therapeutic audio program consisting of multiple layers.
  /// </summary>
  public sealed class FrequencyPreset
  {
    /// <summary>
    /// Unique identifier for this preset.
    /// </summary>
    public required string ID { get; init; }

    /// <summary>
    /// Display name shown to users.
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// Detailed description of the preset's intended effect.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Category this preset belongs to.
    /// </summary>
    public required PresetCategory Category { get; init; }

    /// <summary>
    /// Layer configurations for this preset.
    /// </summary>
    public required List<LayerConfig> Layers { get; init; }

    /// <summary>
    /// Recommended listening duration.
    /// </summary>
    public TimeSpan RecommendedDuration { get; init; } = TimeSpan.FromMinutes(20);

    /// <summary>
    /// Optional tags for searching/filtering.
    /// </summary>
    public List<string> Tags { get; init; } = [];

    /// <summary>
    /// Performs a safety check on all layers to ensure values are within hardware limits.
    /// </summary>
    /// <exception cref="PresetValidationException">Thrown if frequencies or weights are invalid.</exception>
    public void Validate()
    {
      if (Layers.Count == 0)
        throw new PresetValidationException("Preset must contain at least one layer.", ID);

      foreach (LayerConfig layer in Layers)
        if (layer.CarrierHz < 20 || layer.CarrierHz > 20000)
          throw new PresetValidationException($"Invalid carrier frequency: {layer.CarrierHz}", ID);
    }
  }
}
