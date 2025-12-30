using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FreqGen.App.Services;
using FreqGen.Presets.Models;
using FreqGen.Presets.Presets;
using System.Collections.ObjectModel;

namespace FreqGen.App.ViewModels
{
  public sealed partial class MainViewModel(IAudioService audioService) : ObservableObject
  {
    private readonly IAudioService _audioService = audioService;

    [ObservableProperty]
    private string _statusText = "Ready - Select a preset";

    [ObservableProperty]
    private bool _isPlaying;

    [ObservableProperty]
    private FrequencyPreset? _currentPreset;

    public ObservableCollection<PresetGroup> PresetGroups { get; } =
    [
      new(
        "🧠 Brainwave Entrainment",
        PresetEngine.GetPresetsByCategory(PresetCategory.Brainwave)
      ),
      new(
        "🎵 Solfeggio Frequencies",
        PresetEngine.GetPresetsByCategory(PresetCategory.Solfeggio)
      ),
      new(
        "⚡ Isochronic Tones",
        PresetEngine.GetPresetsByCategory(PresetCategory.Isochronic)
      )
    ];

    public async Task InitializeAsync()
    {
      try
      {
        await _audioService.InitializeAsync();
        StatusText = "Ready - Select a preset";
      }
      catch (Exception ex)
      {
        StatusText = $"Initialization failed: {ex.Message}";
        System.Diagnostics.Debug.WriteLine($"Init error: {ex}");
      }
    }

    [RelayCommand]
    private async Task PlayPresetAsync(FrequencyPreset preset)
    {
      if (preset is null)
        return;

      try
      {
        await _audioService.PlayPresetAsync(preset);

        CurrentPreset = preset;
        IsPlaying = true;
        StatusText = $"▶ Playing: {preset.DisplayName}\n{preset.Description}";
      }
      catch (Exception ex)
      {
        StatusText = $"Error: {ex.Message}";
        System.Diagnostics.Debug.WriteLine($"Play error: {ex}");
      }
    }

    [RelayCommand]
    private async Task StopAsync()
    {
      try
      {
        await _audioService.StopAsync();

        IsPlaying = false;
        CurrentPreset = null;
        StatusText = "⏹ Stopped";
      }
      catch (Exception ex)
      {
        StatusText = $"Error: {ex.Message}";
        System.Diagnostics.Debug.WriteLine($"Stop error: {ex}");
      }
    }
  }

  /// <summary>
  /// Grouped collection of presets for display.
  /// </summary>
  public sealed class PresetGroup(string name, IEnumerable<FrequencyPreset> presets)
    : List<FrequencyPreset>(presets)
  {
    public string Name { get; set; } = name;
  }
}