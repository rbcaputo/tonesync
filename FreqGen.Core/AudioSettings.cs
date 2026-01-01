namespace FreqGen.Core
{
  /// <summary>
  /// Provides global constants and default values for the FreqGen audio system.
  /// </summary>
  public static class AudioSettings
  {
    /// <summary>
    /// Default high-fidelity sample rate.
    /// </summary>>
    public const int SampleRate = 44100;

    /// <summary>
    /// Recommended buffer size for mobile low-latency.
    /// </summary>
    public const int BufferSize = 1024;

    /// <summary>
    /// Constraints for Carrier frequencies.
    /// </summary>
    public static class Carrier
    {
      public const float Minimum = 20.0f;
      public const float Maximum = 2000.0f;
      public const float Default = 440.0f;
    }

    /// <summary>
    /// Constraints for Modulation (LFO) frequencies.
    /// </summary>
    public static class Modulation
    {
      public const float Minimum = 0.1f;
      public const float Maximum = 100.0f;
    }

    /// <summary>
    /// Constants for smooth signal transitions.
    /// </summary>
    public static class Envelope
    {
      public const float DefaultAttackSeconds = 10.0f;
      public const float DefaultReleaseSeconds = 10.0f;
    }
  }
}
