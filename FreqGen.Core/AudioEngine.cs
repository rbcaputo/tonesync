using FreqGen.Core.Exceptions;

namespace FreqGen.Core
{
  /// <summary>
  /// The primary entry point for the FreqGen DSP engine.
  /// Coordinates the Mixer and Layers, providing a thread-safe interface for the UI and platform audio.
  /// </summary>
  /// <remarks>
  /// Initializes a new instance of the <see cref="AudioEngine"/> class.
  /// </remarks>
  /// <param name="sampleRate">The target sample rate (e.g., 44100).</param>
  public sealed class AudioEngine(float sampleRate = AudioSettings.SampleRate) : IDisposable
  {
    private readonly Mixer _mixer = new();
    private readonly float _sampleRate = sampleRate;
    private bool _isPlaying;
    private bool _isDisposed;

    /// <summary>
    /// Gets the current sample rate the engine is operating at.
    /// </summary>
    public float SampleRate => _sampleRate;

    /// <summary>
    /// Gets a value indicating whether the engine is currently generating audio.
    /// </summary>
    public bool IsPlaying => _isPlaying;

    /// <summary>
    /// Begins the audio generation process by triggering the envelopes of all active layers.
    /// </summary>
    public void Start()
    {
      ObjectDisposedException.ThrowIf(_isDisposed, this);

      _isPlaying = true;
    }

    /// <summary>
    /// Stops audio generation, triggering the release phase for all layers.
    /// </summary>
    public void Stop() =>
      _isPlaying = false;

    /// <summary>
    /// Fills a provided buffer with generated audio.
    /// This is the 'Hot Path' and must be called by the platform audio callback.
    /// </summary>
    /// <param name="buffer">The span to fill with audio samples.</param>
    /// <param name="configs">The current configurations for the signal path.</param>
    public void FillBuffer(Span<float> buffer, IList<LayerConfiguration> configs)
    {
      ObjectDisposedException.ThrowIf(_isDisposed, this);

      if (!_isPlaying)
      {
        buffer.Clear();
        return;
      }

      try
      {
        // Delegation to block-based processor for SIMD optimization
        _mixer.Render(buffer, _sampleRate, configs);
      }
      catch (Exception ex)
      {
        throw new AudioPlaybackException("Failed to render audio block.", ex);
      }
    }

    /// <summary>
    /// Immediately silences the engine and resets all internal oscillator states.
    /// </summary>
    public void Reset() =>
      _mixer.Reset();

    public void Dispose()
    {
      if (_isDisposed)
        return;

      Stop();
      _mixer.Reset();
      _isDisposed = true;

      GC.SuppressFinalize(this);
    }
  }
}
