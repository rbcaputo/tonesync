using FreqGen.Presets.Models;

namespace FreqGen.Presets.Presets
{
  /// <summary>
  /// Isochronic tone presets using high modulation depth for rhythmic pulsing.
  /// 
  /// IMPLEMENTATION NOTE:
  /// These presets use high-depth amplitude modulation (AM) with sine wave carriers
  /// to create isochronic-like pulses. True isochronic tones use square wave gating,
  /// but high-depth sine wave AM (0.85-1.0) produces similar entrainment effects
  /// with less harmonic distortion and listening fatigue.
  /// 
  /// </summary>
  public static class IsochronicPresets
  {
    public static FrequencyPreset Ic_Delta_PowerNap => new()
    {
      ID = "is_delta_01",
      DisplayName = "PowerNap (3Hz)",
      Description = "Sharp 3Hz pulses for rapid physical recovery and restorative rest.",
      Category = PresetCategory.Isochronic,
      Layers =
        [
          new() { CarrierHz = 120f, ModulationHz = 3.0f, ModulationDepth = 1.0f, Weight = 1.0f }
        ]
    };

    public static FrequencyPreset Ic_Theta_Creative => new()
    {
      ID = "ic_theta_01",
      DisplayName = "Creative Surge (6Hz)",
      Description = "Rhythmic pulsing at 6Hz to bypass analytical filters and access creativity.",
      Category = PresetCategory.Isochronic,
      Layers =
        [
          new() { CarrierHz = 180f, ModulationHz = 6.0f, ModulationDepth = 1.0f, Weight = 1.0f }
        ]
    };

    public static FrequencyPreset Ic_Alpha_Unwind => new()
    {
      ID = "ic_alpha_01",
      DisplayName = "Deep Unwind (10Hz)",
      Description = "10Hz pulses for stress release and clearing mental fog.",
      Category = PresetCategory.Isochronic,
      Layers =
        [
          new() { CarrierHz = 240f, ModulationHz = 10.0f, ModulationDepth = 1.0f, Weight = 1.0f }
        ]
    };

    public static FrequencyPreset Ic_Beta_Focus => new()
    {
      ID = "ic_beta_01",
      DisplayName = "High Focus (15Hz)",
      Description = "Intense 15Hz pulsing for study, coding, or high-performance cognitive work.",
      Category = PresetCategory.Isochronic,
      Layers =
        [
          new() { CarrierHz = 300f, ModulationHz = 15.0f, ModulationDepth = 1.0f, Weight = 1.0f }
        ]
    };

    public static IEnumerable<FrequencyPreset> All =>
      [
        Ic_Delta_PowerNap,
        Ic_Theta_Creative,
        Ic_Alpha_Unwind,
        Ic_Beta_Focus
      ];
  }
}
