using ToneSync.Core;
using ToneSync.Core.Layers;

namespace ToneSync.Presets.Models
{
  /// <summary>
  /// Represents the human-readable configuration for a single audio layer.
  /// Includes metadata for UI display and mapping logic for the Core engine.
  /// Supports both mono and stereo (binaural) configurations.
  /// Immutable to prevent accidental modification of preset definitions.
  /// </summary>
  public sealed record LayerConfig
  {
    /// <summary>
    /// Carrier frequency in Hz (20-2000Hz range).
    /// </summary>
    public required float CarrierHz { get; init; }

    /// <summary>
    /// Modulation frequency in Hz (0 = no modulation, 0.1-100Hz range).
    /// </summary>
    public required float ModulationHz { get; init; }

    /// <summary>
    /// Modulation depth (0.0 = no modulation, 1.0 = full modulation).
    /// </summary>
    public required float ModulationDepth { get; init; }

    /// <summary>
    /// Layer output weight/volume (0.0 = silent, 1.0 = full volume).
    /// </summary>
    public required float Weight { get; init; }

    /// <summary>
    /// Frequency offset between left and right channels (Hz).
    /// Non-zero values create binaural beat effects.
    /// Positive = right channel higher, negative = left channel higher.
    /// </summary>
    public float StereoFrequencyOffset { get; init; } = 0.0f;

    /// <summary>
    /// Pan position for mono layers in stereo output.
    /// -1.0 = full left, 0.0 = center, +1.0 = full right.
    /// Ignored for stereo layers (StereoFrequencyOffset != 0).
    /// </summary>
    public float Pan { get; init; } = 0.0f;

    /// <summary>
    /// Optional human-readable description of this layer's purpose.
    /// Displayed in UI when available.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Maps this domain model to the Core engine's high-performance configuration record.
    /// Performs validation during conversion.
    /// </summary>
    /// <param name="weight">Whether this layer should currently be audible.</param>
    /// <param name="channelMode"></param>
    /// <returns>A validated LayerConfiguration for the audio engine.</returns>
    /// <exception cref="Core.Exceptions.InvalidConfigurationException">Thrown if validation fails.</exception>
    public LayerConfiguration ToCoreConfig(
      float weight,
      ChannelMode channelMode = ChannelMode.Mono
    ) => new(
      CarrierHz,
      ModulationHz,
      ModulationDepth,
      weight,
      channelMode,
      StereoFrequencyOffset,
      Pan
    );

    /// <summary>
    /// Creates a pure tone mono layer configuration (no modulation).
    /// Useful for Solfeggio frequencies and simple carrier tones.
    /// </summary>
    /// <param name="frequency">Carrier frequency in Hz.</param>
    /// <param name="weight">Output volume (default: 1.0).</param>
    /// <param name="pan"></param>
    /// <param name="description">Optional description.</param>
    /// <returns>A LayerConfig with no modulation.</returns>
    public static LayerConfig PureTone(
      float frequency,
      float weight = 1.0f,
      float pan = 0.0f,
      string? description = null
    ) => new()
    {
      CarrierHz = frequency,
      ModulationHz = 0.0f,
      ModulationDepth = 0.0f,
      Weight = weight,
      Pan = pan,
      Description = description ?? "Pure Tone"
    };

    /// <summary>
    /// Creates an amplitude-modulated mono layer configuration.
    /// Useful for brainwave entrainment and therapeutic audio.
    /// </summary>
    /// <param name="carrierHz">Carrier frequency in Hz.</param>
    /// <param name="modulationHz">Modulation frequency in Hz.</param>
    /// <param name="depth">Modulation depth (0.0-1.0).</param>
    /// <param name="weight">Output volume (default: 1.0).</param>
    /// <param name="pan"></param>
    /// <param name="description">Optional description.</param>
    /// <returns>A LayerConfig with amplitude modulation.</returns>
    public static LayerConfig AmTone(
      float carrierHz,
      float modulationHz,
      float depth,
      float weight = 1.0f,
      float pan = 0.0f,
      string? description = null
    ) => new()
    {
      CarrierHz = carrierHz,
      ModulationHz = modulationHz,
      ModulationDepth = depth,
      Weight = weight,
      Pan = pan,
      Description = description ?? $"AM Tone ({modulationHz} Hz)"
    };

    /// <summary>
    /// Creates a binaural beat stereo layer.
    /// The beat frequency is perceived as the difference between left and right carriers.
    /// </summary>
    /// <param name="baseCarrierHz">Base carrier frequency (left channel).</param>
    /// <param name="beatFrequencyHz">Perceived beat frequency (typically 0.5-40 Hz).</param>
    /// <param name="weight">Output volume.</param>
    /// <param name="description">Optional description.</param>
    public static LayerConfig BinauralBeat(
      float baseCarrierHz,
      float beatFrequencyHz,
      float weight = 1.0f,
      string? description = null
    ) => new()
    {
      CarrierHz = baseCarrierHz,
      ModulationHz = 0.0f,
      ModulationDepth = 0.0f,
      Weight = weight,
      StereoFrequencyOffset = beatFrequencyHz,
      Description = description ?? $"Binaural {beatFrequencyHz}Hz"
    };

    /// <summary>
    /// Creates a binaural beat with amplitude modulation.
    /// Combines stereo beat effect with AM modulation.
    /// </summary>
    /// <param name="baseCarrierHz">Base carrier frequency (left channel).</param>
    /// <param name="beatFrequencyHz">Perceived beat frequency (typically 0.5-40 Hz).</param>
    /// <param name="modulationHz">Modulation frequency in Hz.</param>
    /// <param name="depth">Modulation depth (0.0-1.0).</param>
    /// <param name="weight">Output volume.</param>
    /// <param name="description">Optional description.</param>
    public static LayerConfig BinauralAMTone(
      float baseCarrierHz,
      float beatFrequencyHz,
      float modulationHz,
      float depth,
      float weight,
      string? description = null
    ) => new()
    {
      CarrierHz = baseCarrierHz,
      ModulationHz = modulationHz,
      ModulationDepth = depth,
      Weight = weight,
      StereoFrequencyOffset = beatFrequencyHz,
      Description = description ?? $"Binaural {beatFrequencyHz}Hz + AM {modulationHz}Hz"
    };
  }
}
