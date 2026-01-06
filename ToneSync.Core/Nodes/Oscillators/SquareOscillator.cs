using ToneSync.Core.Nodes;

namespace ToneSync.Core.Nodes.Oscillators
{
  /// <summary>
  /// A square wave oscillator for isochronic tones.
  /// Provides hard transitions suitable for rhythmic brainwave entrainment.
  /// </summary>
  public sealed class SquareOscillator : IAudioNode
  {
    private float _phase;
    private float _phaseIncrement;

    /// <summary>
    /// Gets or sets the duty cycle (0.0 to 1.0). Default is 0.5 (symmetric square).
    /// </summary>
    public float DutyCycle { get; set; } = 0.5f;

    /// <summary>
    /// Set the oscillator frequency.
    /// Safe to call from any thread.
    /// </summary>
    public void SetFrequency(float frequency, float sampleRate) =>
      _phaseIncrement = frequency / sampleRate;

    /// <summary>
    /// Processes the buffer with square pulses.
    /// </summary>
    public void Process(Span<float> buffer)
    {
      for (int i = 0; i < buffer.Length; i++)
      {
        buffer[i] = _phase < DutyCycle ? 1.0f : -1.0f;
        _phase = (_phase + _phaseIncrement) % 1.0f;
      }
    }

    /// <summary>
    /// Resets the oscillator state to prevent clicks on restart.
    /// </summary>
    public void Reset() =>
      _phase = 0f;
  }
}
