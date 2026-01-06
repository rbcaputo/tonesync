using FreqGen.Core.Layers;
using System.Runtime.CompilerServices;

namespace FreqGen.Core
{
  /// <summary>
  /// Sums multiple audio layers into a single interleaved output buffer.
  /// All layers are pre-allocated to ensure allocation-free execution inside the
  /// real-time audio callback.
  /// 
  /// This class performs pure additive mixing only.
  /// Gain staging, normalization, and safety limiting are handled by
  /// higher-level components.
  /// </summary>
  public sealed class Mixer
  {
    /// <summary>
    /// Fixed attenuation applied after summing all active layers.
    /// 
    /// This reserves headroom to prevent clipping when multiple layers,
    /// envelopes, and modulators are active simultaneously.
    /// 
    /// A value of 0.5 corresponds to -6 dB of headroom.
    /// </summary>
    private const float MixHeadroom = 0.5f;

    /// <summary>
    /// Fixed-size pool of audio layers used for rendering.
    /// The array is allocated once and never resized to guarantee
    /// deterministic behavior on the audio thread.
    /// </summary>
    private readonly Layer[] _layers = new Layer[AudioSettings.MaxLayers];

    /// <summary>
    /// Temporary buffer used for rendering individual layers before
    /// summing them into the final output buffer.
    /// This buffer may be resized during initialization but is
    /// never allocated during steady-state audio processing.
    /// </summary>
    private float[] _tempBuffer = new float[AudioSettings.MaxBufferSize];

    /// <summary>
    /// Number of currently active layers initialized in the mixer.
    /// This value is fixed after initialization and
    /// read frequently by the audio thread.
    /// </summary>
    private int _activeLayerCount;

    /// <summary>
    /// Gets the current number of active layers.
    /// </summary>
    public int ActiveLayerCount => _activeLayerCount;

    /// <summary>
    /// Initializes the mixer with a fixed number of layers.
    /// Must be called once before first use.
    /// </summary>
    /// <param name="layerCount">Number of layers to allocate (max 8).</param>
    /// <param name="sampleRate">System audio sample rate.</param>
    /// <param name="attackSeconds">Envelope attack time for all layers.</param>
    /// <param name="releaseSeconds">Envelope release time for all layers.</param>
    /// <remarks>
    /// This method must be called exactly once before the mixer is used.
    /// It is not thread-safe and must not be called while audio rendering is in progress.
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown if layerCount exceeds max.</exception>
    public void Initialize(
      int layerCount, float sampleRate,
      float attackSeconds = AudioSettings.EnvelopeSettings.DefaultAttackSeconds,
      float releaseSeconds = AudioSettings.EnvelopeSettings.DefaultReleaseSeconds
    )
    {
      if (layerCount <= 0 || layerCount > AudioSettings.MaxLayers)
        throw new ArgumentException(
          $"Layer count must be between 1 and {AudioSettings.MaxLayers}. Got: {layerCount}",
          nameof(layerCount));

      _activeLayerCount = layerCount;

      // Pre-allocate all layers
      for (int i = 0; i < layerCount; i++)
      {
        _layers[i] = new Layer();
        _layers[i].Initialize(sampleRate, attackSeconds, releaseSeconds);
      }
    }

    /// <summary>
    /// Renders and mixes all active audio layers into the provided output buffer.
    /// </summary>
    /// <param name="outputBuffer">
    /// Destination buffer that will contain the summed audio signal.
    /// The buffer is always fully written.
    /// </param>
    /// <param name="sampleRate">System audio sample rate.</param>
    /// <param name="configs">Read-only snapshot of per-layer configuration data.</param>
    /// <remarks>
    /// This method is part of the real-time audio path:
    /// - No locks
    /// - No allocations during steady-state operation
    /// - Deterministic execution time
    ///
    /// The mixer performs additive summation only.
    /// No gain normalization, limiting, or safety clamping is applied here.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void Render(
      Span<float> outputBuffer,
      float sampleRate,
      ReadOnlySpan<LayerConfiguration> configs
    )
    {
      // Clear output buffer
      outputBuffer.Clear();

      // Resize temp buffer if needed (should only happen once at startup)
      if (_tempBuffer.Length < outputBuffer.Length)
        _tempBuffer = new float[outputBuffer.Length];

      // Render each active layer
      int layersToRender = Math.Min(_activeLayerCount, configs.Length);

      for (int i = 0; i < layersToRender; i++)
      {
        Span<float> tempSpan = _tempBuffer.AsSpan(0, outputBuffer.Length);
        tempSpan.Clear();

        // Render layer into temporary buffer
        _layers[i].UpdateAndProcess(tempSpan, sampleRate, configs[i]);

        // Additively Mix into output buffer
        MixBuffers(outputBuffer, tempSpan);
      }

      // Apply fixed mix headroom attenuation
      ApplyHeadroom(outputBuffer);
    }

    /// <summary>
    /// Adds the contents of one buffer into another using sample-wise addition.
    /// Assumes both buffers are the same length.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void MixBuffers(Span<float> destination, ReadOnlySpan<float> source)
    {
      for (int i = 0; i < destination.Length; i++)
        destination[i] += source[i];
    }

    /// <summary>
    /// Applies fixed post-mix attenuation to reserve headroom
    /// before device-specific gain and safety limiting.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ApplyHeadroom(Span<float> buffer)
    {
      for (int i = 0; i < buffer.Length; i++)
        buffer[i] *= MixHeadroom;
    }

    /// <summary>
    /// Gets the current envelope value for a specific layer.
    /// Useful for UI metering.
    /// </summary>
    /// <param name="layerIndex">Zero-based layer index.</param>
    /// <returns>Envelope value (0.0 to 1.0), or 0.0 if index is invalid.</returns>
    public float GetLayerEnvelopeValue(int layerIndex)
    {
      if (layerIndex < 0 || layerIndex >= _activeLayerCount)
        return 0.0f;

      return _layers[layerIndex].CurrentEnvelopeValue;
    }

    /// <remarks>
    /// This method does not stop audio immediately.
    /// Each layer will fade out according to its configured release time.
    /// </remarks>
    public void TriggerReleaseAll()
    {
      for (int i = 0; i < _activeLayerCount; i++)
        _layers[i].TriggerRelease();
    }

    /// <summary>
    /// Resets all layers to prevent clicks on restart.
    /// Should be called when audio engine is fully stopped.
    /// </summary>
    /// <remarks>
    /// This method resets internal oscillator and envelope state.
    /// It should only be called when the audio engine is fully stopped,
    /// as it may cause discontinuities if used during active playback.
    /// </remarks>
    public void Reset()
    {
      for (int i = 0; i < _activeLayerCount; i++)
        _layers[i].Reset();
    }
  }
}
