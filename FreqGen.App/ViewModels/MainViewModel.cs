using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FreqGen.App.Services;
using FreqGen.Presets.Models;
using FreqGen.Presets.Presets;
using System.Collections.ObjectModel;

namespace FreqGen.App.ViewModels
{
  public sealed partial class MainViewModel(IAudioService audioService)
    : ObservableObject, IDisposable
  {
    private readonly IAudioService _audioService = audioService;

    [ObservableProperty]
    private string _statusText = "Initializing...";

    [ObservableProperty]
    private bool _isPlaying;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private string? _errorMessage;

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
      if (IsLoading)
        return;

      try
      {
        IsLoading = true;
        HasError = false;
        ErrorMessage = null;
        StatusText = "Initializing audio system...";

        await _audioService.InitializeAsync();

        StatusText = "Ready - Select a preset to begin";
        IsLoading = false;
      }
      catch (Exception ex)
      {
        HasError = true;
        ErrorMessage = ex.Message;
        StatusText = $"Initialization failed: {ex.Message}";
        IsLoading = false;

        System.Diagnostics.Debug.WriteLine($"Init error: {ex}");
      }
    }

    [RelayCommand(CanExecute = nameof(CanPlayPreset))]
    private async Task PlayPresetAsync(FrequencyPreset? preset)
    {
      if (preset is null || IsLoading)
        return;

      try
      {
        IsLoading = true;
        HasError = false;
        ErrorMessage = null;
        StatusText = $"Loading {preset.DisplayName}...";

        await _audioService.PlayPresetAsync(preset);

        CurrentPreset = preset;
        IsPlaying = true;
        StatusText = $"▶ Playing: {preset.DisplayName}\n{preset.Description}";
        IsLoading = false;
      }
      catch (Exception ex)
      {
        HasError = true;
        ErrorMessage = ex.Message;
        StatusText = $"Error: {ex.Message}";
        IsPlaying = false;
        IsLoading = false;

        System.Diagnostics.Debug.WriteLine($"Play error: {ex}");
      }
    }

    private bool CanPlayPreset() => !IsLoading && !IsPlaying;

    [RelayCommand(CanExecute = nameof(CanStop))]
    private async Task StopAsync()
    {
      if (IsLoading)
        return;

      try
      {
        IsLoading = true;
        StatusText = "Stopping...";

        await _audioService.StopAsync();

        IsPlaying = false;
        CurrentPreset = null;
        StatusText = "⏹ Stopped - Select a preset to begin";
        IsLoading = false;
      }
      catch (Exception ex)
      {
        HasError = true;
        ErrorMessage = ex.Message;
        StatusText = $"Error: {ex.Message}";
        IsLoading = false;

        System.Diagnostics.Debug.WriteLine($"Stop error: {ex}");
      }
    }

    private bool CanStop() => IsPlaying && !IsLoading;

    [RelayCommand]
    private async Task RetryInitializationAsync()
    {
      if (IsLoading)
        return;

      try
      {
        IsLoading = true;
        HasError = false;
        ErrorMessage = null;
        StatusText = "Retrying initialization...";

        bool success = await _audioService.RetryInitializationAsync();

        if (success)
        {
          StatusText = "Ready - Select a preset to begin";
          HasError = false;
        }
        else
        {
          HasError = true;
          ErrorMessage = "Retry failed";
          StatusText = "Retry failed - Please restart the app";
        }

        IsLoading = false;
      }
      catch (Exception ex)
      {
        HasError = true;
        ErrorMessage = ex.Message;
        StatusText = $"Retry failed: {ex.Message}";
        IsLoading = false;

        System.Diagnostics.Debug.WriteLine($"Retry error: {ex}");
      }
    }

    [RelayCommand]
    private void DismissError()
    {
      HasError = false;
      ErrorMessage = null;

      if (!IsPlaying)
        StatusText = "Ready - Select a preset to begin";
    }

    public void Dispose()
    {
      // ViewModel cleanup
      // AudioService disposal is handled by DI container
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
