using FreqGen.Presets.Models;

namespace FreqGen.Presets.Presets
{
  /// <summary>
  /// Isochronic tone presets using high modulation depth for rhythmic pulsing.
  /// More pronounced than standard AM - creates distinct on/off pulses.
  /// </summary>
  public static class IsochronicPresets
  {
    /// <summary>
    /// Deep Relaxation (4 Hz Isochronic)
    /// Strong theta pulses for deep relaxation.
    /// </summary>
    public static FrequencyPreset DeepRelaxation { get; } = new()
    {
      ID = "isochronic_theta_4hz",
      DisplayName = "Deep Relaxation (4 Hz)",
      Description = "Isochronic theta pulses for deep relaxation",
      Category = PresetCategory.Isochronic,
      RecommendedDuration = TimeSpan.FromMinutes(20),
      Tags = ["isochronic", "theta", "relaxation", "pulses"],
      Layers =
      [
        new LayerConfig
        {
          CarrierHz = 250f,
          ModulationHz = 4f,
          ModulationDepth = 1f, // Full depth for clear pulses
          Weight = 1f,
          Description = "4 Hz theta pulses"
        }
      ]
    };

    public static FrequencyPreset MeditationBoost { get; } = new()
    {
      ID = "isochronic_theta_7hz",
      DisplayName = "Meditation Boost (7 Hz)",
      Description = "Isochronic theta pulses to enhance meditation depth",
      Category = PresetCategory.Isochronic,
      RecommendedDuration = TimeSpan.FromMinutes(25),
      Tags = ["isochronic", "meditation", "theta"],
      Layers =
      [
        new LayerConfig
        {
          CarrierHz = 220f,
          ModulationHz = 7f,
          ModulationDepth = 0.95f,
          Weight = 1f,
          Description = "7 Hz theta pulses"
        }
      ]
    };

    /// <summary>
    /// Focus Training (10 Hz Isochronic)
    /// Alpha-range pulses for sustained focus and concentration.
    /// </summary>
    public static FrequencyPreset FocusTraining { get; } = new()
    {
      ID = "isochronic_alpha_10hz",
      DisplayName = "Focus Training (10 Hz)",
      Description = "Isochronic alpha pulses for focus and concentration",
      Category = PresetCategory.Isochronic,
      RecommendedDuration = TimeSpan.FromMinutes(30),
      Tags = ["isochronic", "focus", "alpha", "concentration"],
      Layers =
      [
        new LayerConfig
        {
          CarrierHz = 300f,
          ModulationHz = 10f,
          ModulationDepth = 0.9f,
          Weight = 1f,
          Description = "10 Hz alpha pulses"
        }
      ]
    };

    /// <summary>
    /// Energy Boost (15 Hz Isochronic)
    /// Beta-range pulses for mental alertness and energy.
    /// </summary>
    public static FrequencyPreset EnergyBoost { get; } = new()
    {
      ID = "isochronic_beta_15hz",
      DisplayName = "Energy Boost (15 Hz)",
      Description = "Isochronic beta pulses for alertness and energy",
      Category = PresetCategory.Isochronic,
      RecommendedDuration = TimeSpan.FromMinutes(20),
      Tags = ["isochronic", "energy", "beta", "alertness"],
      Layers =
      [
        new LayerConfig
        {
          CarrierHz = 280f,
          ModulationHz = 15f,
          ModulationDepth = 0.85f,
          Weight = 1f,
          Description = "15 Hz beta pulses"
        }
      ]
    };

    /// <summary>
    /// Power Nap (3 Hz Isochronic)
    /// Deep delta pulses for quick, restorative rest.
    /// </summary>
    public static FrequencyPreset PowerNap { get; } = new()
    {
      ID = "isochronic_delta_3hz",
      DisplayName = "Power Nap (3 Hz)",
      Description = "Deep delta pulses for quick restorative sleep",
      Category = PresetCategory.Isochronic,
      RecommendedDuration = TimeSpan.FromMinutes(15),
      Tags = ["isochronic", "sleep", "delta", "nap"],
      Layers =
      [
        new LayerConfig
        {
          CarrierHz = 180f,
          ModulationHz = 3f,
          ModulationDepth = 1f,
          Weight = 1f,
          Description = "3 Hz delta pulses"
        }
      ]
    };

    /// <summary>
    /// All isochronic presets.
    /// </summary>
    public static FrequencyPreset[] All { get; } =
    [
      DeepRelaxation,
      MeditationBoost,
      FocusTraining,
      EnergyBoost,
      PowerNap
    ];
  }
}
