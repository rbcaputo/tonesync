using FreqGen.Presets.Models;

namespace FreqGen.Presets.Presets
{
  /// <summary>
  /// Solfeggio frequency presets.
  /// Based on the ancient 6-tone scale used in sacred music.
  /// </summary>
  public static class SolfeggioPresets
  {
    /// <summary>
    /// 396 Hz - Liberation from Fear
    /// Associated with releasing fear and guilt.
    /// </summary>
    public static FrequencyPreset Liberation { get; } = new()
    {
      ID = "solfeggio_396",
      DisplayName = "Liberation (396 Hz)",
      Description = "Release fear, guilt, and negative emotions",
      Category = PresetCategory.Solfeggio,
      RecommendedDuration = TimeSpan.FromMinutes(20),
      Tags = ["solfeggio", "fear", "liberation", "healing"],
      Layers =
      [
        LayerConfig.PureTone(
          frequency: 396f,
          weight: 1f,
          description: "Root tone"
        )
      ]
    };

    /// <summary>
    /// 417 Hz - Facilitating Change
    /// Associated with undoing situations and facilitating change.
    /// </summary>
    public static FrequencyPreset Change { get; } = new()
    {
      ID = "solfeggio_417",
      DisplayName = "Facilitating Change (417 Hz)",
      Description = "Undo negative situations and facilitate positive change",
      Category = PresetCategory.Solfeggio,
      RecommendedDuration = TimeSpan.FromMinutes(20),
      Tags = ["solfeggio", "change", "transformation"],
      Layers =
      [
        LayerConfig.PureTone(
          frequency: 417f,
          weight: 1f,
          description: "Root tone"
        )
      ]
    };

    /// <summary>
    /// 528 Hz - Transformation and DNA Repair
    /// The "Love Frequency" - associated with healing and transformation.
    /// </summary>
    public static FrequencyPreset Transformation { get; } = new()
    {
      ID = "solfeggio_528",
      DisplayName = "Transformation (528 Hz)",
      Description = "The Love Frequency - healing, transformation, and DNA repair",
      Category = PresetCategory.Solfeggio,
      RecommendedDuration = TimeSpan.FromMinutes(20),
      Tags = ["solfeggio", "love", "healing", "transformation", "dna"],
      Layers =
      [
        new LayerConfig
        {
          CarrierHz = 528f,
          ModulationHz = 8f, // Light alpha modulation
          ModulationDepth = 0.3f,
          Weight = 1f,
          Description = "528 Hz with gentle alpha modulation"
        }
      ]
    };

    /// <summary>
    /// 639 Hz - Connecting and Relationships
    /// Associated with harmonious relationships and communication.
    /// </summary>
    public static FrequencyPreset Connection { get; } = new()
    {
      ID = "solfeggio_639",
      DisplayName = "Connection (639 Hz)",
      Description = "Harmonious relationships and heart-centered communication",
      Category = PresetCategory.Solfeggio,
      RecommendedDuration = TimeSpan.FromMinutes(20),
      Tags = ["solfeggio", "relationships", "connection", "heart"],
      Layers =
      [
        LayerConfig.PureTone(
          frequency: 639f,
          weight: 1f,
          description: "Root tone"
        )
      ]
    };

    /// <summary>
    /// 741 Hz - Expression and Solutions
    /// Associated with self-expression and problem-solving.
    /// </summary>
    public static FrequencyPreset Expression { get; } = new()
    {
      ID = "solfeggio_741",
      DisplayName = "Connection (741 Hz)",
      Description = "Authentic self-expression and finding solutions",
      Category = PresetCategory.Solfeggio,
      RecommendedDuration = TimeSpan.FromMinutes(20),
      Tags = ["solfeggio", "expression", "clarity", "solutions"],
      Layers =
      [
        LayerConfig.PureTone(
          frequency: 741f,
          weight: 1f,
          description: "Root tone"
        )
      ]
    };

    /// <summary>
    /// 852 Hz - Awakening Intuition
    /// Associated with spiritual awareness and intuition.
    /// </summary>
    public static FrequencyPreset Intuition { get; } = new()
    {
      ID = "solfeggio_852",
      DisplayName = "Intuition (852 Hz)",
      Description = "Awakening intuition and spiritual awareness",
      Category = PresetCategory.Solfeggio,
      RecommendedDuration = TimeSpan.FromMinutes(20),
      Tags = ["solfeggio", "intuition", "spiritual", "awareness"],
      Layers =
      [
        LayerConfig.PureTone(
          frequency: 852f,
          weight: 1f,
          description: "Root tone"
        )
      ]
    };

    /// <summary>
    /// Complete Solfeggio Sequence
    /// All six frequencies played in harmony (use with care - complex sound).
    /// </summary>
    public static FrequencyPreset CompleteHarmony { get; } = new()
    {
      ID = "solfeggio_complete",
      DisplayName = "Complete Harmony",
      Description = "All six Solfeggio frequencies in balanced harmony",
      Category = PresetCategory.Solfeggio,
      RecommendedDuration = TimeSpan.FromMinutes(30),
      Tags = ["solfeggio", "complete", "harmony", "advanced"],
      Layers =
      [
        LayerConfig.PureTone(396f, 0.17f, "Liberation"),
        LayerConfig.PureTone(417f, 0.17f, "Change"),
        LayerConfig.PureTone(528f, 0.20f, "Transformation (emphasized)"),
        LayerConfig.PureTone(639f, 0.17f, "Connection"),
        LayerConfig.PureTone(741f, 0.15f, "Expression"),
        LayerConfig.PureTone(852f, 0.14f, "Intuition")
      ]
    };

    /// <summary>
    /// All Solfeggio presets.
    /// </summary>
    public static FrequencyPreset[] All { get; } =
    [
      Liberation,
      Change,
      Transformation,
      Connection,
      Expression,
      Intuition,
      CompleteHarmony
    ];
  }
}
