using FreqGen.Core.Nodes;
using FreqGen.Core.Nodes.Modulators;
using FreqGen.Core.Nodes.Oscillators;

namespace FreqGen.Core
{
  /// <summary>
  /// Orchestrates a single signal path: Oscillator -> Modulator -> Envelope -> Mixer.
  /// </summary>
  public sealed class Layer
  {
    private readonly SineOscillator _carrier = new();
    private readonly LFO _lfo = new();
    private readonly Envelope _envelope = new();
    private readonly float[] _modulatorBuffer = new float[512];

    /// <summary>
    /// Processes the layer logic into the provided buffer based on a configuration.
    /// </summary>
    /// <param name="buffer">The buffer to update.</param>
    /// <param name="sampleRate">System audio sample rate.</param>
    /// <param name="config">The configurations for the layer.</param>
    public void UpdateAndProcess(Span<float> buffer, float sampleRate, LayerConfiguration config)
    {
      _carrier.SetFrequency(config.CarrierFrequency, sampleRate);
      _lfo.SetFrequency(config.ModulatorFrequency, sampleRate);
      _envelope.Trigger(config.IsActive, sampleRate, 30.0f); // 30s

      // Generate carrier
      _carrier.Process(buffer);

      // Generate modulator signal
      Span<float> modulatorSpan = _modulatorBuffer.AsSpan(0, buffer.Length);
      _lfo.Process(modulatorSpan);

      // Apply AM
      AMModulator.Apply(buffer, modulatorSpan, config.ModulatorDepth);

      // Apply weight and envelope
      for (int i = 0; i < buffer.Length; i++)
        buffer[i] *= config.Weight;

      _envelope.Process(buffer);
    }

    /// <summary>
    /// Resets the layer state to prevent clicks on restart.
    /// </summary>
    public void Reset()
    {
      _carrier.Reset();
      _lfo.Reset();
      _envelope.Reset();
    }
  }
}
