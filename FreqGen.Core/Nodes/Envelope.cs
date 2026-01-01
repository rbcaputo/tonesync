namespace FreqGen.Core.Nodes
{
  /// <summary>
  /// An exponential envelope generator for smooth signal transitions.
  /// Crucial for preventing startle responses in therapeutic audio.
  /// </summary>
  public sealed class Envelope : IAudioNode
  {
    private float _current;
    private float _target;
    private float _coefficient;

    /// <summary>
    /// Triggers the envelope to fade in (true) or out (false).
    /// </summary>
    public void Trigger(bool active, float sampleRate, float timeInSeconds)
    {
      _target = active ? 1.0f : 0.0f;
      _coefficient = 1.0f / (MathF.Max(0.001f, timeInSeconds) * sampleRate);
    }

    /// <summary>
    /// Applies the envelope gain to the provided buffer.
    /// </summary>
    public void Process(Span<float> buffer)
    {
      for (int i = 0; i < buffer.Length; i++)
      {
        _current += (_target - _current) * _coefficient;
        buffer[i] *= _current;
      }
    }

    /// <summary>
    /// Resets the envelope state to prevent clicks on restart.
    /// </summary>
    public void Reset()
    {
      _current = 0f;
      _target = 0f;
    }
  }
}
