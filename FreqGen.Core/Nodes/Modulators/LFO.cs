namespace FreqGen.Core.Nodes.Modulators
{
  /// <summary>
  /// Low-Frequency Oscillator optimized for modulation.
  /// Updates at a control rate (every 64 samples) to conserve CPU on mobile devices.
  /// </summary>
  public sealed class LFO : IAudioNode
  {
    private const int ControlRate = 64; // Update every 64 samples

    private float _phase;
    private float _phaseIncrement;
    private float _currentValue;
    private int _sampleCounter;

    /// <summary>
    /// Set the LFO frequency (typically 0.5-30 Hz)
    /// </summary>
    public void SetFrequency(float frequency, float sampleRate) =>
      // Compensate increment for the reduced update rate
      _phaseIncrement = MathF.Tau * frequency / (sampleRate / ControlRate);

    /// <summary>
    /// Populates the buffer with a stepped LFO signal for modulation.
    /// </summary>
    public void Process(Span<float> buffer)
    {
      for (int i = 0; i < buffer.Length; i++)
      {
        if (_sampleCounter++ % ControlRate == 0)
        {
          _currentValue = MathF.Sin(_phase);
          _phase = (_phase + _phaseIncrement) % MathF.Tau;
        }

        buffer[i] = _currentValue;
      }
    }

    /// <summary>
    /// Resets the oscillator state to prevent clicks on restart.
    /// </summary>
    public void Reset()
    {
      _phase = 0f;
      _currentValue = 0f;
      _sampleCounter = 0;
    }
  }
}
