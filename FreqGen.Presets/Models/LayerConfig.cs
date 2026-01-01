using FreqGen.Core;

namespace FreqGen.Presets.Models
{
  /// <summary>
  /// Represents the human-readable configuration for a single audio layer.
  /// Includes metadata for UI display and mapping logic for the Core engine.
  /// </summary>
  public sealed record LayerConfig
  {
    /// <summary>
    /// Carrier frequency in Hz.
    /// </summary>
    public required float CarrierHz { get; init; }

    /// <summary>
    /// Modulation frequency in Hz (0 = no modulation).
    /// </summary>
    public required float ModulationHz { get; init; }

    /// <summary>
    /// Modulation depth (0.0-1.0).
    /// </summary>
    public required float ModulationDepth { get; init; }

    /// <summary>
    /// Layer output weight/volume (0.0-1.0).
    /// </summary>
    public required float Weight { get; init; }

    /// <summary>
    /// Optional description of this layer's purpose.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Maps this domain model to the Core engine's high-performance configuration record.
    /// </summary>
    /// <param name="isActive">Whether this layer should currently be audible.</param>
    public LayerConfiguration ToCoreConfig(bool isActive) =>
      new(CarrierHz, ModulationHz, ModulationDepth, Weight, isActive);

    public static LayerConfig PureTone(float frequency, float weight = 1.0f) =>
      new()
      {
        CarrierHz = frequency,
        ModulationHz = 0f,
        ModulationDepth = 0f,
        Weight = weight,
        Description = "Fundamental Pure Tone"
      };
  }
}
