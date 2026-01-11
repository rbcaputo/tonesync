using ToneSync.Core.Exceptions;

namespace ToneSync.Core.Layers
{
  /// <summary>
  /// Immutable, validated configuration data for a single audio layer.
  /// All parameters are guaranteed to be within safe operating ranges.
  /// </summary>
  public readonly record struct LayerConfiguration
  {
    private readonly float _carrierFrequency;
    private readonly float _modulatorFrequency;
    private readonly float _modulatorDepth;
    private readonly float _weight;

    /// <summary>
    /// Gets the carrier (audible tone) frequency in Hz.
    /// </summary>
    public float CarrierFrequency => _carrierFrequency;

    /// <summary>
    /// Gets the modulation (LFO) frequency in Hz.
    /// Set to 0 for pure tone (no modulation).
    /// </summary>
    public float ModulatorFrequency => _modulatorFrequency;

    /// <summary>
    /// Gets the modulation depth (0.0 = no modulation, 1.0 = full modulation).
    /// </summary>
    public float ModulatorDepth => _modulatorDepth;

    /// <summary>
    /// Gets the layer output weight/volume (0.0 = silent, 1.0 = full volume).
    /// </summary>
    public float Weight => _weight;

    /// <summary>
    /// Channel routing for this layer.
    /// </summary>
    public ChannelMode ChannelMode { get; init; }

    /// <summary>
    /// Frequency offset between left and right channels (for binaural beats).
    /// Only applies when ChannelMode is Stereo.
    /// Positive values = right channel higher, negative = left channel higher.
    /// </summary>
    public float StereoFrequencyOffset { get; init; }

    /// <summary>
    /// Pan position (-1.0 = full left, 0.0 = center, 1.0 = full right).
    /// Only applies when ChannelMode is Mono (panned to stereo output).
    /// </summary>
    public float Pan { get; init; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="carrierFrequency"></param>
    /// <param name="modulatorFrequency"></param>
    /// <param name="modulatorDepth"></param>
    /// <param name="weight"></param>
    /// <param name="channelMode"></param>
    /// <param name="steroFrequencyOffset"></param>
    /// <param name="pan"></param>
    /// <exception cref="InvalidConfigurationException"></exception>
    public LayerConfiguration(
      float carrierFrequency,
      float modulatorFrequency,
      float modulatorDepth,
      float weight,
      ChannelMode channelMode = ChannelMode.Mono,
      float steroFrequencyOffset = 0.0f,
      float pan = 0.0f
    )
    {
      if (!AudioSettings.CarrierSettings.IsValid(carrierFrequency, AudioSettings.SampleRate))
        throw new InvalidConfigurationException(
          $"Carrier frequency {carrierFrequency} Hz is outside valid range " +
          $"({AudioSettings.CarrierSettings.Minimum}-{AudioSettings.CarrierSettings.Maximum} Hz) " +
          $"or too close to Nyquist frequency.",
          nameof(CarrierFrequency)
        );

      if (!AudioSettings.ModulationSettings.IsValid(modulatorFrequency))
        throw new InvalidConfigurationException(
          $"Modulator frequency {modulatorFrequency} Hz is outside valid range " +
          $"({AudioSettings.ModulationSettings.Minimum}-{AudioSettings.ModulationSettings.Maximum}Hz).",
          nameof(ModulatorFrequency)
        );

      if (!AudioSettings.AmplitudeSettings.IsValid(modulatorDepth))
        throw new InvalidConfigurationException(
          $"Modulator depth {modulatorDepth} is outside valid range " +
          $"({AudioSettings.AmplitudeSettings.Minimum}-{AudioSettings.AmplitudeSettings.Maximum}).",
          nameof(ModulatorDepth)
        );

      if (!AudioSettings.AmplitudeSettings.IsValid(weight))
        throw new InvalidConfigurationException(
          $"Layer weight {weight} is outside valid range " +
          $"({AudioSettings.AmplitudeSettings.Minimum}-{AudioSettings.AmplitudeSettings.Maximum}).",
          nameof(Weight)
        );

      if (channelMode == ChannelMode.Stereo && Math.Abs(steroFrequencyOffset) > 0.001f)
      {
        float rightFrequency = carrierFrequency + steroFrequencyOffset;
        if (!AudioSettings.CarrierSettings.IsValid(rightFrequency, AudioSettings.SampleRate))
          throw new InvalidConfigurationException(
            $"Right channel frequency ({rightFrequency}Hz) after stereo offset " +
            $"is outside valid range or too close to Nyquist frequency.",
            nameof(StereoFrequencyOffset)
          );
      }

      ChannelMode = channelMode;
      StereoFrequencyOffset = steroFrequencyOffset;
      Pan = pan;

      _carrierFrequency = carrierFrequency;
      _modulatorFrequency = modulatorFrequency;
      _modulatorDepth = modulatorDepth;
      _weight = weight;
    }

    /// <summary>
    /// Validates this configuration against the current sample rate.
    /// </summary>
    /// <param name="sampleRate">The system's audio sample rate.</param>
    /// <exception cref="InvalidConfigurationException">Thrown if validation fails.</exception>
    public void ValidateForSampleRate(float sampleRate)
    {
      if (!AudioSettings.CarrierSettings.IsValid(_carrierFrequency, sampleRate))
        throw new InvalidConfigurationException(
          $"Carrier frequency {_carrierFrequency} Hz exceeds Nyquist limit " +
          $"for sample rate {sampleRate}Hz.",
          nameof(CarrierFrequency)
        );
    }
  }
}
