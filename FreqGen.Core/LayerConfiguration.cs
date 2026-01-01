namespace FreqGen.Core
{
  /// <summary>
  /// Immutable configuration data for a single audio layer.
  /// </summary>
  public record LayerConfiguration(
    float CarrierFrequency,
    float ModulatorFrequency,
    float ModulatorDepth,
    float Weight,
    bool IsActive
  );
}
