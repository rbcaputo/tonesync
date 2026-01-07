using ToneSync.Core;
using ToneSync.Core.Layers;

namespace ToneSync.Tests.Core
{
  public sealed class MixerTests
  {
    [Fact]
    public void Render_Fully_Writes_Output_Buffer()
    {
      var mixer = new Mixer();
      mixer.Initialize(2, AudioSettings.SampleRate);

      var buffer = new float[AudioSettings.RecommendedBufferSize / 2];
      Array.Fill(buffer, float.NaN);

      mixer.Render(
        buffer,
        AudioSettings.SampleRate,
        SilentConfigs(2)
      );

      foreach (var sample in buffer)
        Assert.False(float.IsNaN(sample));
    }

    [Fact]
    public void Output_Is_Attenuated()
    {
      var mixer = new Mixer();
      mixer.Initialize(4, AudioSettings.SampleRate);

      var buffer = new float[AudioSettings.RecommendedBufferSize];

      mixer.Render(
        buffer,
        AudioSettings.SampleRate,
        SilentConfigs(4)
      );

      foreach (var sample in buffer)
        Assert.InRange(sample, -1f, 1f);
    }

    [Fact]
    public void Does_Not_Generate_NaNs_Or_Infinities()
    {
      var mixer = new Mixer();
      mixer.Initialize(8, AudioSettings.SampleRate);

      var buffer = new float[AudioSettings.RecommendedBufferSize];

      for (var i = 0; i < 1000; i++)
      {
        mixer.Render(
          buffer,
          AudioSettings.SampleRate,
          SilentConfigs(8)
        );

        foreach (var sample in buffer)
        {
          Assert.False(float.IsNaN(sample));
          Assert.False(float.IsInfinity(sample));
        }
      }
    }

    [Fact]
    public void Get_Layer_Envelope_Value_Is_Bounds_Safe()
    {
      var mixer = new Mixer();
      mixer.Initialize(2, AudioSettings.SampleRate);

      Assert.Equal(0f, mixer.GetLayerEnvelopeValue(-1));
      Assert.Equal(0f, mixer.GetLayerEnvelopeValue(99));
    }

    [Fact]
    public void Trigger_Release_All_Is_Safe()
    {
      var mixer = new Mixer();
      mixer.Initialize(3, AudioSettings.SampleRate);

      mixer.TriggerReleaseAll();

      var buffer = new float[AudioSettings.RecommendedBufferSize / 2];
      mixer.Render(
        buffer,
        AudioSettings.SampleRate,
        SilentConfigs(3)
      );

      foreach (var sample in buffer)
        Assert.False(float.IsNaN(sample));
    }

    [Fact]
    public void Reset_Is_Safe_And_Idempotent()
    {
      var mixer = new Mixer();
      mixer.Initialize(2, AudioSettings.SampleRate);

      mixer.Reset();
      mixer.Reset();

      var buffer = new float[AudioSettings.RecommendedBufferSize / 2];
      mixer.Render(
        buffer,
        AudioSettings.SampleRate,
        SilentConfigs(2)
      );

      foreach (var sample in buffer)
        Assert.False(float.IsNaN(sample));
    }

    [Fact]
    public void Is_Deterministic()
    {
      var mixerA = new Mixer();
      var mixerB = new Mixer();
      mixerA.Initialize(3, AudioSettings.SampleRate);
      mixerB.Initialize(3, AudioSettings.SampleRate);

      var bufferA = new float[AudioSettings.RecommendedBufferSize];
      var bufferB = new float[AudioSettings.RecommendedBufferSize];

      for (var i = 0; i < 20; i++)
      {
        mixerA.Render(
          bufferA,
          AudioSettings.SampleRate,
          SilentConfigs(3)
        );
        mixerB.Render(
          bufferB,
          AudioSettings.SampleRate,
          SilentConfigs(3)
        );

        for (var j = 0; j < bufferA.Length; j++)
          Assert.Equal(bufferA[j], bufferB[j], 1e-6f);
      }
    }

    private static LayerConfiguration[] SilentConfigs(int count)
    {
      var configs = new LayerConfiguration[count];
      for (int i = 0; i < count; i++)
        configs[i] = default!; // silence-safe by design

      return configs;
    }
  }
}
