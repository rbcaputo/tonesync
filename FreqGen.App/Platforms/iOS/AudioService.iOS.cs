using AudioToolbox;
using AVFoundation;
using Foundation;

namespace FreqGen.App.Services
{
  /// <summary>
  /// iOS-specific audio implementation using AVAudioEngine.
  /// </summary>
  public sealed partial class AudioService
  {
    private AVAudioEngine? _avAudioEngine;
    private AVAudioSourceNode? _sourceNode;
    private AVAudioFormat? _audioFormat;

    partial void InitializePlatformAudio()
    {
      // Configure audio session
      AVAudioSession audioSession = AVAudioSession.SharedInstance();
      audioSession.SetCategory(AVAudioSessionCategory.Playback);
      audioSession.SetActive(true, out NSError error);

      if (error is not null)
        throw new InvalidOperationException($"Failed to activate audio session: {error}");

      // Create audio engine
      _avAudioEngine = new();

      // Create audio format (mono 44.1kHz, float32)
      _audioFormat = new(
        sampleRate: FreqGen.Core.AudioSettings.SampleRate,
        channels: 1
      );

      if (_audioFormat is null)
        throw new InvalidOperationException("Failed to create audio format");

      // Create source node
      _sourceNode = new(_audioFormat, (isSilencePtr, timestampPtr, frameCount, audioBufferListPtr) =>
      {
        unsafe
        {
          bool isSilence = *(bool*)isSilencePtr;
          AudioTimeStamp timestamp = *(AudioTimeStamp*)timestampPtr;
          AudioBufferList audioBufferList = *(AudioBufferList*)audioBufferListPtr;

          return RenderAudio(isSilence, timestamp, frameCount, audioBufferList);
        }
      });

      // Connect nodes
      _avAudioEngine.AttachNode(_sourceNode);
      _avAudioEngine.Connect(
        _sourceNode,
        _avAudioEngine.MainMixerNode,
        _audioFormat
      );

      // Prepare engine
      _avAudioEngine.Prepare();
    }

    partial void StartPlatformAudio()
    {
      if (_avAudioEngine is null || _avAudioEngine.Running)
        return;

      try
      {
        _avAudioEngine.StartAndReturnError(out NSError error);

        if (error is not null)
          throw new InvalidOperationException($"Failed to start audio engine: {error}");
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine($"iOS audio start failed: {ex}");
        throw;
      }
    }

    partial void StopPlatformAudio()
    {
      if (_avAudioEngine is null || !_avAudioEngine.Running)
        return;

      _avAudioEngine.Stop();
    }

    partial void DisposePlatformAudio()
    {
      if (_avAudioEngine?.Running == true)
        _avAudioEngine.Stop();

      _sourceNode?.Dispose();
      _avAudioEngine?.Dispose();

      _sourceNode = null;
      _avAudioEngine = null;
      _audioFormat = null;
    }

    private unsafe int RenderAudio(
      bool isSilence,
      AudioTimeStamp timeStamp,
      uint frameCount,
      AudioBufferList audioBufferList
    )
    {
      if (_engine is null || isSilence)
        return 0;

      try
      {
        // Get the audio buffer
        AudioBuffer* buffer = audioBufferList.GetBuffer(0);
        float* floatPtr = (float*)buffer->Data;

        // Fill temporary buffer from engine
        float[] tempBuffer = [frameCount];
        _engine.FillBuffer(tempBuffer);

        // Copy to output buffer
        for (int i = 0; i < frameCount; i++)
          floatPtr[i] = tempBuffer[i];

        return 0;
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine($"iOS render error: {ex}");
        return -1;
      }
    }
  }
}
