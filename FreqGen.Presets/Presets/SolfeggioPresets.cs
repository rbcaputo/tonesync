using FreqGen.Presets.Models;

namespace FreqGen.Presets.Presets
{
  /// <summary>
  /// Solfeggio frequency presets.
  /// Based on the ancient 6-tone scale used in sacred music.
  /// </summary>
  public static class SolfeggioPresets
  {
    public static FrequencyPreset Sf_174Hz => new()
    {
      ID = "sf_174",
      DisplayName = "Foundation (174Hz)",
      Description = "Associated with pain reduction and organizational safety.",
      Category = PresetCategory.Solfeggio,
      Layers = [LayerConfig.PureTone(174f)]
    };

    public static FrequencyPreset Sf_285Hz => new()
    {
      ID = "sf_285",
      DisplayName = "Quantum Cognition (285Hz)",
      Description = "Associated with healing tissue and internal organ repair.",
      Category = PresetCategory.Solfeggio,
      Layers = [LayerConfig.PureTone(285f)]
    };

    public static FrequencyPreset Sf_396Hz => new()
    {
      ID = "sf_396",
      DisplayName = "Liberation (396Hz)",
      Description = "Associated with turning grief into joy and liberating guilt/fear.",
      Category = PresetCategory.Solfeggio,
      Layers = [LayerConfig.PureTone(396f)]
    };

    public static FrequencyPreset Sf_417Hz => new()
    {
      ID = "sf_417",
      DisplayName = "Change (417Hz)",
      Description = "Associated with undoing situations and facilitating positive change.",
      Category = PresetCategory.Solfeggio,
      Layers = [LayerConfig.PureTone(417f)]
    };

    public static FrequencyPreset Sf_528Hz => new()
    {
      ID = "sf_528",
      DisplayName = "Transformation (528Hz)",
      Description = "The 'Miracle' tone for DNA repair, transformation, and clarity of mind.",
      Category = PresetCategory.Solfeggio,
      Layers = [LayerConfig.PureTone(528f)]
    };

    public static FrequencyPreset Sf_639Hz => new()
    {
      ID = "sf_639",
      DisplayName = "Relationship (639Hz)",
      Description = "Associated with harmonizing relationships and social connection.",
      Category = PresetCategory.Solfeggio,
      Layers = [LayerConfig.PureTone(639f)]
    };

    public static FrequencyPreset Sf_741Hz => new()
    {
      ID = "sf_741",
      DisplayName = "Expression (741Hz)",
      Description = "Associated with solving problems and expression/solutions.",
      Category = PresetCategory.Solfeggio,
      Layers = [LayerConfig.PureTone(741f)]
    };

    public static FrequencyPreset Sf_852Hz => new()
    {
      ID = "sf_852",
      DisplayName = "Intuition (852Hz)",
      Description = "Associated with returning to spiritual order and awakening intuition.",
      Category = PresetCategory.Solfeggio,
      Layers = [LayerConfig.PureTone(852f)]
    };

    public static FrequencyPreset Sf_963Hz => new()
    {
      ID = "sf_963",
      DisplayName = "Divinity (963Hz)",
      Description = "Associated with higher consciousness and spiritual enlightenment.",
      Category = PresetCategory.Solfeggio,
      Layers = [LayerConfig.PureTone(963f)]
    };

    /// <summary>
    /// A complex harmony of the core solfeggio triad (396, 528, 639).
    /// </summary>
    public static FrequencyPreset Sf_Harmony => new()
    {
      ID = "sf_harmony",
      DisplayName = "The Core Triad Harmony",
      Description = "A balanced mix of Liberation, Transformation, and Connection.",
      Category = PresetCategory.Solfeggio,
      Layers =
        [
          LayerConfig.PureTone(396f, 0.33f),
          LayerConfig.PureTone(528f, 0.33f),
          LayerConfig.PureTone(639f, 0.33f)
        ]
    };

    public static IEnumerable<FrequencyPreset> All =>
      [
        Sf_174Hz, Sf_285Hz, Sf_396Hz,
        Sf_417Hz, Sf_528Hz, Sf_639Hz,
        Sf_741Hz, Sf_852Hz, Sf_963Hz,
        Sf_Harmony
      ];
  }
}
