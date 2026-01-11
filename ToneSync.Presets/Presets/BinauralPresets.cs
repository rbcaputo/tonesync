using ToneSync.Presets.Models;

namespace ToneSync.Presets.Presets
{
  /// <summary>
  /// Binaural beat presets using stereo frequency offsets.
  /// 
  /// IMPORTANT: These presets REQUIRE HEADPHONES for proper effect.
  /// Binaural beats work through neural interference - the brain perceives the
  /// difference between two slightly different frequencies played separately to each ear.
  /// 
  /// The beat frequency is never present in the audio signal itself,
  /// but emerges as a perceptual phenomenon in the auditory cortex.
  /// </summary>
  public static class BinauralPresets
  {
    /// <summary>
    /// Delta binaural beat (2.5Hz) - Deep relaxation and sleep.
    /// Base carrier at 200Hz with 2.5Hz offset.
    /// </summary>
    public static FrequencyPreset Bn_Delta_DeepSleep => new()
    {
      ID = "bn_delta_01",
      DisplayName = "Deep Sleep (2.5Hz Binaural)",
      Description = "Binaural delta beat at 2.5Hz promotes deep sleep and physical restoration. " +
                    "The brain perceives a 2.5Hz rhythm from the frequency difference between ears. " +
                    "REQUIRES HEADPHONES.",
      Category = PresetCategory.Binaural,
      IsBeginnerFriendly = true,
      RecommendedDuration = TimeSpan.FromMinutes(30),
      Tags = ["binaural", "sleep", "delta", "deep-rest", "headphones"],
      Layers =
        [
          LayerConfig.BinauralBeat(
            carrierFrequency: 200f,
            beatFrequency: 2.5f,
            weight: 1.0f,
            description: "Delta binaural beat"
          )
        ]
    };

    /// <summary>
    /// Theta binaural beat (6Hz) - Meditation and creativity.
    /// Base carrier at 250Hz with 6Hz offset.
    /// </summary>
    public static FrequencyPreset Bn_Theta_Meditation => new()
    {
      ID = "bn_theta_01",
      DisplayName = "Deep Meditation(6Hz Binaural)",
      Description = "Binaural theta beat at 6Hz facilitates deep meditation and creative insight. " +
                    "The 6Hz perceived rhythm bridges conscious and unconscious states. " +
                    "REQUIRES HEADPHONES.",
      Category = PresetCategory.Binaural,
      IsBeginnerFriendly = true,
      RecommendedDuration = TimeSpan.FromMinutes(20),
      Tags = ["binaural", "meditation", "theta", "creativity", "headphones"],
      Layers =
        [
          LayerConfig.BinauralBeat(
            carrierFrequency: 250f,
            beatFrequency: 6.0f,
            weight: 1.0f,
            description: "Theta binaural beat"
          )
        ]
    };

    /// <summary>
    /// Alpha binaural beat (10Hz) - Relaxed focus and stress relief.
    /// Base carrier at 300Hz with 10Hz offset.
    /// </summary>
    public static FrequencyPreset Bn_Alpha_Focus => new()
    {
      ID = "bn_alpha_01",
      DisplayName = "Relaxed Focus (10Hz Binaural)",
      Description = "Binaural alpha beat at 10Hz promotes relaxed alertness and stress reduction. " +
                    "The 10Hz rhythm is associated with 'flow states' and effortless concentration. " +
                    "REQUIRES HEADPHONES.",
      Category = PresetCategory.Binaural,
      IsBeginnerFriendly = true,
      RecommendedDuration = TimeSpan.FromMinutes(25),
      Tags = ["binaural", "focus", "alpha", "stress-relief", "headphones"],
      Layers =
        [
          LayerConfig.BinauralBeat(
            carrierFrequency: 300f,
            beatFrequency: 10.0f,
            weight: 1.0f,
            description: "Alpha binaural beat"
          )
        ]
    };

    /// <summary>
    /// Beta binaural beat (18Hz) - Active concentration.
    /// Base carrier at 350Hz with 18Hz offset.
    /// </summary>
    public static FrequencyPreset Bn_Beta_Concentration => new()
    {
      ID = "bn_beta_01",
      DisplayName = "Active Concentration (18Hz Binaural)",
      Description = "Binaural beta beat at 18Hz enhances active thinking and analytical processing. " +
                     "The 18Hz rhythm supports sustained cognitive performance. " +
                     "REQUIRES HEADPHONES.",
      Category = PresetCategory.Binaural,
      IsBeginnerFriendly = false,
      RecommendedDuration = TimeSpan.FromMinutes(20),
      Tags = ["binaural", "concentration", "beta", "cognitive", "headphones"],
      Layers =
        [
          LayerConfig.BinauralBeat(
            carrierFrequency: 350f,
            beatFrequency: 18.0f,
            weight: 1.0f,
            description: "Beta binaural beat"
          )
        ]
    };

    /// <summary>
    /// Gamma binaural beat (40Hz) - Peak awareness.
    /// Base carrier at 400Hz with 40Hz offset.
    /// </summary>
    public static FrequencyPreset Bn_Gamma_Awareness => new()
    {
      ID = "bn_gamma_01",
      DisplayName = "Peak Awareness (40Hz Binaural)",
      Description = "Binaural gamma beat at 40Hz associated with heightened perception and insight. " +
                    "The 40Hz rhythm correlates with high-level information processing. " +
                    "Use for short sessions only. REQUIRES HEADPHONES.",
      Category = PresetCategory.Binaural,
      IsBeginnerFriendly = false,
      RecommendedDuration = TimeSpan.FromMinutes(10),
      Tags = ["binaural", "gamma", "awareness", "advanced", "headphones"],
      Layers =
        [
          LayerConfig.BinauralBeat(
            carrierFrequency: 400f,
            beatFrequency: 40.0f,
            weight: 1.0f,
            description: "Gamma binaural beat"
          )
        ]
    };

    /// <summary>
    /// SMR binaural beat (13Hz) - Calm alertness.
    /// Base carrier at 280Hz with 13Hz offset.
    /// Perfect for sustained focus without mental fatigue.
    /// </summary>
    public static FrequencyPreset Bn_SMR_CalmAlert => new()
    {
      ID = "bn_smr_01",
      DisplayName = "Calm Alertness (13Hz Binaural)",
      Description = "Binaural SMR beat at 13Hz promotes relaxed yet alert state. " +
                    "The sensorimotor rhythm is ideal for sustained focus without tension. " +
                    "REQUIRES HEADPHONES.",
      Category = PresetCategory.Binaural,
      IsBeginnerFriendly = true,
      RecommendedDuration = TimeSpan.FromMinutes(30),
      Tags = ["binaural", "smr", "calm", "focus", "headphones"],
      Layers =
        [
          LayerConfig.BinauralBeat(
            carrierFrequency: 280f,
            beatFrequency: 13.0f,
            weight: 1.0f,
            description: "SMR binaural beat"
          )
        ]
    };

    /// <summary>
    /// Dual-layer binaural preset combining theta and alpha frequencies.
    /// Base carriers at 220Hz and 330Hz with respective beat offsets.
    /// Creates a rich, complex entrainment experience.
    /// </summary>
    public static FrequencyPreset Bn_Dual_CreativeFocus => new()
    {
      ID = "bn_dual_01",
      DisplayName = "Creative Focus (Dual Binaural)",
      Description = "Combines theta (6Hz) and alpha (10Hz) binaural beats for balanced " +
                    "creativity and focus. The dual-layer approach creates a rich entrainment " +
                    "experience supporting both intuitive and analytical thinking. " +
                    "REQUIRES HEADPHONES.",
      Category = PresetCategory.Binaural,
      IsBeginnerFriendly = true,
      RecommendedDuration = TimeSpan.FromMinutes(25),
      Tags = ["binaural", "dual-layer", "creative", "focus", "headphones"],
      Layers =
        [
          LayerConfig.BinauralBeat(
            carrierFrequency: 220f,
            beatFrequency: 6.0f,
            weight: 0.6f,
            description: "Theta binaural component"
          ),
          LayerConfig.BinauralBeat(
            carrierFrequency: 330f,
            beatFrequency: 10.0f,
            weight: 0.4f,
            description: "Alpha binaural component"
          )
        ]
    };

    /// <summary>
    /// Binaural beat with amplitude modulation overlay.
    /// Combines the stereo beat effect with rhythmic AM pulsing.
    /// Advanced preset for experienced users.
    /// </summary>
    public static FrequencyPreset Bn_Alpha_Enhanced => new()
    {
      ID = "bn_alpha_am_01",
      DisplayName = "Enhanced Alpha (Binaural + AM)",
      Description = "Alpha binaural beat (10Hz) enhanced with subtle amplitude modulation (10Hz). " +
                    "The combination of binaural and AM techniques creates a powerful " +
                    "multi-dimensional entrainment effect. REQUIRES HEADPHONES.",
      Category = PresetCategory.Binaural,
      IsBeginnerFriendly = false,
      RecommendedDuration = TimeSpan.FromMinutes(20),
      Tags = ["binaural", "alpha", "enhanced", "advanced", "headphones"],
      Layers =
        [
          LayerConfig.BinauralAMTone(
            carrierFrequency: 300f,
            beatFrequency: 10.0f,
            modulationFrequency: 10.0f,
            depth: 0.4f,
            weight: 1.0f,
            description: "Binaural + AM alpha enhancement"
          )
        ]
    };

    /// <summary>
    /// Ultra-deep delta binaural (1Hz) for advanced meditation.
    /// Extremely slow beat for profound states of consciousness.
    /// </summary>
    public static FrequencyPreset Bn_Delta_UltraDeep => new()
    {
      ID = "bb_delta_ultra_01",
      DisplayName = "Ultra-Deep State (1Hz Binaural)",
      Description = "Ultra-slow binaural delta beat at 1Hz for advanced meditation and " +
                     "exploration of deep consciousness states. This very slow rhythm " +
                     "may facilitate profound relaxation and altered states. " +
                     "REQUIRES HEADPHONES. For experienced meditators.",
      Category = PresetCategory.Binaural,
      IsBeginnerFriendly = false,
      RecommendedDuration = TimeSpan.FromMinutes(30),
      Tags = ["binaural", "delta", "ultra-deep", "advanced", "meditation", "headphones"],
      Layers =
        [
          LayerConfig.BinauralBeat(
            carrierFrequency: 150f,
            beatFrequency: 1.0f,
            weight: 1.0f,
            description: "Ultra-slow delta binaural"
          )
        ]
    };

    /// <summary>
    /// Progressive binaural session that could be extended with automation.
    /// Currently static at alpha frequency, but demonstrates multi-layer design.
    /// </summary>
    public static FrequencyPreset Bn_Progressive_Relaxation => new()
    {
      ID = "bn_progressive_01",
      DisplayName = "Progressive Relaxation (Multi-Stage)",
      Description = "Multi-layer binaural preset designed for progressive relaxation. " +
                    "Combines alpha (10Hz) and theta (5Hz) beats at different intensities " +
                    "to create a layered entrainment experience. REQUIRES HEADPHONES.",
      Category = PresetCategory.Binaural,
      IsBeginnerFriendly = true,
      RecommendedDuration = TimeSpan.FromMinutes(30),
      Tags = ["binaural", "progressive", "relaxation", "multi-stage", "headphones"],
      Layers =
        [
          LayerConfig.BinauralBeat(
            carrierFrequency: 300f,
            beatFrequency: 10.0f,
            weight: 0.7f,
            description: "Primary alpha beat"
          ),
          LayerConfig.BinauralBeat(
            carrierFrequency: 240f,
            beatFrequency: 5.0f,
            weight: 0.3f,
            description: "Supporting theta beat"
          )
        ]
    };

    /// <summary>
    /// Gets all binaural beat presets.
    /// </summary>
    public static IReadOnlyList<FrequencyPreset> All =>
      [
        Bn_Delta_DeepSleep,
        Bn_Delta_UltraDeep,
        Bn_Theta_Meditation,
        Bn_Alpha_Focus,
        Bn_Alpha_Enhanced,
        Bn_SMR_CalmAlert,
        Bn_Beta_Concentration,
        Bn_Gamma_Awareness,
        Bn_Dual_CreativeFocus,
        Bn_Progressive_Relaxation
      ];
  }
}
