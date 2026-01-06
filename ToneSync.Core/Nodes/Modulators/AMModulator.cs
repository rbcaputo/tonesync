using System.Runtime.CompilerServices;

namespace ToneSync.Core.Nodes.Modulators
{
  /// <summary>
  /// Applies headroom-safe amplitude modulation (AM) to an audio buffer.
  /// 
  /// This implementation guarantees that modulation never increases the
  /// signal above unity gain, preventing clipping and intermodulation artifacts when
  /// multiple layers are summed.
  /// </summary>
  public static class AMModulator
  {
    /// <summary>
    /// Applies amplitude modulation to the carrier buffer using a modulator buffer.
    /// The modulation is scaled to preserve headroom and avoid gain overshoot.
    /// </summary>
    /// <param name="carrier">
    /// The audio buffer containing the carrier signal. Modified in place.
    /// Expected range: [-1, 1].
    /// </param>
    /// <param name="modulator">
    /// The modulation buffer (typically an LFO).
    /// Expected range: [-1, 1].
    /// </param>
    /// <param name="depth">
    /// Modulation depth in the range [0, 1].
    /// A value of 1 produces full-depth AM without exceeding unity gain.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Apply(
      Span<float> carrier,
      ReadOnlySpan<float> modulator,
      float depth
    )
    {
      if (depth <= 0)
        return;

      // Clamp depth defensively (hot path safe)
      if (depth > 1f)
        depth = 1f;

      for (int i = 0; i < carrier.Length; i++)
      {
        float modulation = modulator[i]; // [-1, 1]

        // Headroom safe AM
        // Maps modulation into [1 - depth, 1]
        float amplitude = 1f - depth + (depth * 0.5f * (modulation + 1f));

        carrier[i] *= amplitude;
      }
    }
  }
}
