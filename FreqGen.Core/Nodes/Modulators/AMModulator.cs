namespace FreqGen.Core.Nodes.Modulators
{
  /// <summary>
  /// Static processor for applying Amplitude Modulation (AM).
  /// Implements the formula: Carrier * (1 + Depth * Modulator).
  /// </summary>
  public sealed class AMModulator
  {
    /// <summary>
    /// Gets or sets the intensity of the modulation effect (0.0 to 1.0).
    /// </summary>
    public float Depth { get; set; } = 0.5f;

    /// <summary>
    /// Applies amplitude modulation to an existing audio buffer.
    /// </summary>
    /// <param name="audioBuffer">The buffer containing carrier signal (modified in place).</param>
    /// <param name="modulatorBuffer">The buffer containing the LFO/Modulator signal.</param>
    /// <param name="depth">The intensity of modulation (0.0 to 1.0).</param>
    public static void Apply(Span<float> audioBuffer, ReadOnlySpan<float> modulatorBuffer, float depth)
    {
      for (int i = 0; i < audioBuffer.Length; i++)
        // Ensures signal stays positive and follows the reference formula
        audioBuffer[i] *= 1.0f + (depth * modulatorBuffer[i]);
    }
  }
}
