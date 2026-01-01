using FreqGen.Presets.Models;

namespace FreqGen.Presets.Presets
{
  /// <summary>
  /// Brainwave entrainment presets using amplitude modulation.
  /// Based on established brainwave frequency ranges.
  /// </summary>
  public static class BrainwavePresets
  {
    public static FrequencyPreset Bw_Delta_DeepRest => new()
    {
      ID = "bw_delta_01",
      DisplayName = "Deep Rest (Delta)",
      Description = "2.0 Hz: Ideal for deep sleep, pain relief, and access to the unconscious mind.",
      Category = PresetCategory.Brainwave,
      Layers =
        [
          new() { CarrierHz = 174f, ModulationHz = 2.0f, ModulationDepth = 0.8f, Weight = 0.7f },
          new() { CarrierHz = 176f, ModulationHz = 2.0f, ModulationDepth = 0.4f, Weight = 0.3f }
        ]
    };

    public static FrequencyPreset Bw_Theta_Meditation => new()
    {
      ID = "bw_theta_01",
      DisplayName = "Deep Meditation (Theta)",
      Description = "5.5 Hz: The 'twilight' state between sleep and wakefulness. Promotes visualization and creativity.",
      Category = PresetCategory.Brainwave,
      Layers =
        [
          new() { CarrierHz = 220f, ModulationHz = 5.5f, ModulationDepth = 0.7f, Weight = 0.8f },
          new() { CarrierHz = 225f, ModulationHz = 5.5f, ModulationDepth = 0.5f, Weight = 0.2f }
        ]
    };

    public static FrequencyPreset Bw_Alpha_Relaxation => new()
    {
      ID = "bw_alpha_01",
      DisplayName = "Relaxed Focus (Alpha)",
      Description = "10.0 Hz: Synchronizes the brain for 'flow states,' stress reduction, and light learning.",
      Category = PresetCategory.Brainwave,
      Layers =
        [
          new() { CarrierHz = 330f, ModulationHz = 10.0f, ModulationDepth = 0.6f, Weight = 1.0f }
        ]
    };

    public static FrequencyPreset Bw_Beta_Cognition { get; } = new()
    {
      ID = "bw_beta_01",
      DisplayName = "High Alertness (Beta)",
      Description = "20.0 Hz: Stimulates cognitive processing, concentration, and analytical thinking.",
      Category = PresetCategory.Brainwave,
      Layers =
        [
          new() { CarrierHz = 440f, ModulationHz = 20.0f, ModulationDepth = 0.5f, Weight = 1.0f }
        ]
    };

    public static FrequencyPreset Bw_Gamma_Insight => new()
    {
      ID = "bw_gamma_01",
      DisplayName = "Peak Perception (Gamma)",
      Description = "40.0 Hz: Associated with high-level information processing and bursts of insight.",
      Category = PresetCategory.Brainwave,
      Layers =
        [
          new() { CarrierHz = 512f, ModulationHz = 40.0f, ModulationDepth = 0.3f, Weight = 1.0f }
        ]
    };

    public static IEnumerable<FrequencyPreset> All =>
      [
        Bw_Delta_DeepRest,
        Bw_Theta_Meditation,
        Bw_Alpha_Relaxation,
        Bw_Beta_Cognition,
        Bw_Gamma_Insight,
      ];
  }
}
