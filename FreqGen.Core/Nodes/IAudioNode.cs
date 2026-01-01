namespace FreqGen.Core.Nodes
{
  /// <summary>
  /// Defines the contract for all audio processing nodes within the FreqGen engine.
  /// Supports high-performance block processing to minimize virtual call overhead and enable SIMD.
  /// </summary>
  public interface IAudioNode
  {
    /// <summary>
    /// Processes a block of audio data into the provided buffer.
    /// </summary>
    /// <param name="buffer">The memory span to populate or modify with audio samples.</param>
    public void Process(Span<float> buffer);

    /// <summary>
    /// Resets the internal state (phase, filters, etc.) to zero.
    /// </summary>
    void Reset();
  }
}
