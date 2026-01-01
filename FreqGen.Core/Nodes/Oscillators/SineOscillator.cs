using System.Runtime.CompilerServices;

namespace FreqGen.Core.Nodes.Oscillators
{
  /// <summary>
  /// A high-precision sine wave oscillator.
  /// Uses a phase-accumulator approach to ensure continuous signals across block boundaries.
  /// </summary>
  public sealed class SineOscillator : IAudioNode
  {
    private float _phase;
    private float _phaseIncrement;

    /// <summary>
    /// Updates the oscillator's frequency based on the current sample rate.
    /// </summary>
    /// <param name="frequency">Target frequency in Hz.</param>
    /// <param name="sampleRate">The system's current audio sample rate.</param>
    public void SetFrequency(float frequency, float sampleRate) =>
      _phaseIncrement = MathF.Tau * frequency / sampleRate;

    /// <summary>
    /// Fills the buffer with a pure sine wave.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void Process(Span<float> buffer)
    {
      for (int i = 0; i < buffer.Length; i++)
      {
        buffer[i] = MathF.Sin(_phase);
        _phase = (_phase + _phaseIncrement) % MathF.Tau;
      }
    }

    /// <summary>
    /// Resets the oscillator state to prevent clicks on restart.
    /// </summary>
    public void Reset() =>
      _phase = 0f;
  }
}
