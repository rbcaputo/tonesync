using FreqGen.Presets.Models;

namespace FreqGen.Presets.Presets
{
  /// <summary>
  /// Brainwave entrainment presets using amplitude modulation.
  /// Based on established brainwave frequency ranges.
  /// </summary>
  public static class BrainwavePresets
  {
    /// <summary>
    /// Deep Sleep (Delta: 0.5-4 Hz)
    /// Promotes deep, restorative sleep and physical healing.
    /// </summary>
    public static FrequencyPreset DeepSleep { get; } = new()
    {
      ID = "brainwave_delta_sleep",
      DisplayName = "Deep Sleep",
      Description = "Delta waves (2 Hz) for deep, restorative sleep",
      Category = PresetCategory.Brainwave,
      RecommendedDuration = TimeSpan.FromMinutes(60),
      Tags = ["sleep", "delta", "relaxation", "healing"],
      Layers =
      [
        LayerConfig.BrainwaveLayer(
          carrierHz: 150f,
          brainwaveHz: 2f,
          depth: 0.9f,
          weight: 0.8f,
          description: "Primary delta carrier"
        ),
        LayerConfig.BrainwaveLayer(
          carrierHz: 200f,
          brainwaveHz: 2.5f,
          depth: 0.7f,
          weight: 0.3f,
          description: "Harmonic support"
        )
      ]
    };

    /// <summary>
    /// Deep Meditation (Theta: 4-8 Hz)
    /// Promotes deep meditation, creativity, and intuition.
    /// </summary>
    public static FrequencyPreset DeepMeditation { get; } = new()
    {
      ID = "brainwave_theta_meditation",
      DisplayName = "Deep Meditation",
      Description = "Theta waves (6 Hz) for deep meditation and creativity",
      Category = PresetCategory.Brainwave,
      RecommendedDuration = TimeSpan.FromMinutes(20),
      Tags = ["meditation", "theta", "creativity", "intuition"],
      Layers =
      [
        LayerConfig.BrainwaveLayer(
          carrierHz: 200f,
          brainwaveHz: 6f,
          depth: 0.8f,
          weight: 0.7f,
          description: "Primary theta carrier"
        ),
        LayerConfig.BrainwaveLayer(
          carrierHz: 300f,
          brainwaveHz: 6f,
          depth: 0.6f,
          weight: 0.3f,
          description: "Harmonic support"
        )
      ]
    };

    /// <summary>
    /// Relaxed Focus (Alpha: 8-13 Hz)
    /// Promotes relaxed alertness and calm focus.
    /// </summary>
    public static FrequencyPreset RelaxedFocus { get; } = new()
    {
      ID = "brainwave_alpha_relaxed",
      DisplayName = "Relaxed Focus",
      Description = "Alpha waves (10 Hz) for calm, relaxed awareness",
      Category = PresetCategory.Brainwave,
      RecommendedDuration = TimeSpan.FromMinutes(30),
      Tags = ["focus", "alpha", "relaxation", "awareness"],
      Layers =
      [
        LayerConfig.BrainwaveLayer(
          carrierHz: 250f,
          brainwaveHz: 10f,
          depth: 0.7f,
          weight: 1f,
          description: "Alpha frequency"
        )
      ]
    };

    /// <summary>
    /// Active Focus (Beta: 13-30 Hz)
    /// Promotes concentration, alertness, and active thinking.
    /// </summary>
    public static FrequencyPreset ActiveFocus { get; } = new()
    {
      ID = "brainwave_beta_focus",
      DisplayName = "Active Focus",
      Description = "Beta waves (18 Hz) for concentration and alertness",
      Category = PresetCategory.Brainwave,
      RecommendedDuration = TimeSpan.FromMinutes(45),
      Tags = ["focus", "beta", "concentration", "alertness"],
      Layers =
      [
        LayerConfig.BrainwaveLayer(
          carrierHz: 250f,
          brainwaveHz: 18f,
          depth: 0.5f,
          weight: 1f,
          description: "Beta frequency stimulation"
        )
      ]
    };

    /// <summary>
    /// Peak Performance (Gamma: 30-100 Hz)
    /// Promotes high-level cognitive processing and peak mental performance.
    /// Use sparingly - can be fatiguing.
    /// </summary>
    public static FrequencyPreset PeakPerformance { get; } = new()
    {
      ID = "brainwave_gamma_peak",
      DisplayName = "Peak Performance",
      Description = "Gamma waves (40 Hz) for peak cognitive performance",
      Category = PresetCategory.Brainwave,
      RecommendedDuration = TimeSpan.FromMinutes(15),
      Tags = ["focus", "gamma", "performance", "cognition"],
      Layers =
      [
        LayerConfig.BrainwaveLayer(
          carrierHz: 300f,
          brainwaveHz: 40f,
          depth: 0.3f, // Lower depth to avoid fatigue
          weight: 1f,
          description: "Gamma frequency (use sparingly)"
        )
      ]
    };

    /// <summary>
    /// Study Session (Beta + Alpha blend)
    /// Balanced mix for learning and retention.
    /// </summary>
    public static FrequencyPreset StudySession { get; } = new()
    {
      ID = "brainwave_study_blend",
      DisplayName = "Study Session",
      Description = "Beta-Alpha blend for optimal learning and retention",
      Category = PresetCategory.Brainwave,
      RecommendedDuration = TimeSpan.FromMinutes(60),
      Tags = ["study", "learning", "focus", "memory"],
      Layers =
      [
        LayerConfig.BrainwaveLayer(
         carrierHz: 220f,
         brainwaveHz: 15f, // Beta
         depth: 0.6f,
         weight: 0.6f,
         description: "Beta for focus"
        ),
        LayerConfig.BrainwaveLayer(
          carrierHz: 280f,
          brainwaveHz: 10f, // Alpha
          depth: 0.5f,
          weight: 0.4f,
          description: "Alpha for relaxed learning"
        )
      ]
    };

    /// <summary>
    /// All brainwave presets.
    /// </summary>
    public static FrequencyPreset[] All { get; } =
    [
      DeepSleep,
      DeepMeditation,
      RelaxedFocus,
      ActiveFocus,
      PeakPerformance,
      StudySession
    ];
  }
}
