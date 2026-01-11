using ToneSync.Core.Layers;

namespace ToneSync.Core.Tests
{
  public sealed class MixerTests
  {
    [Fact]
    public void Initialize_Defaults_To_Mono_Mode()
    {
      var mixer = new Mixer();
      mixer.Initialize(2, AudioSettings.SampleRate);

      Assert.Equal(ChannelMode.Mono, mixer.OutputMode);
    }

    [Fact]
    public void Initialize_With_Stereo_Mode_Sets_Output_Mode()
    {
      var mixer = new Mixer();
      mixer.Initialize(2, AudioSettings.SampleRate, ChannelMode.Stereo);

      Assert.Equal(ChannelMode.Stereo, mixer.OutputMode);
    }

    [Fact]
    public void Render_Mono_With_Stereo_Mixer_Throws()
    {
      var mixer = new Mixer();
      mixer.Initialize(2, AudioSettings.SampleRate, ChannelMode.Stereo);

      var buffer = new float[AudioSettings.RecommendedBufferSize / 2];

      Assert.Throws<InvalidOperationException>(() =>
        mixer.RenderMono(buffer, AudioSettings.SampleRate, SilentConfigs(2))
      );
    }

    [Fact]
    public void Render_Stereo_With_Mismatched_Buffer_Sizes_Throws()
    {
      var mixer = new Mixer();
      mixer.Initialize(2, AudioSettings.SampleRate, ChannelMode.Stereo);

      var leftBuffer = new float[AudioSettings.RecommendedBufferSize / 4];
      var rightBuffer = new float[AudioSettings.RecommendedBufferSize / 2];

      Assert.Throws<ArgumentException>(() =>
        mixer.RenderStereo(
          leftBuffer,
          rightBuffer,
          AudioSettings.SampleRate,
          SilentConfigs(2)
        )
      );
    }

    [Fact]
    public void Render_Mono_Fully_Writes_Output_Buffer()
    {
      var mixer = new Mixer();
      mixer.Initialize(2, AudioSettings.SampleRate);

      var buffer = new float[AudioSettings.RecommendedBufferSize / 2];
      Array.Fill(buffer, float.NaN);

      mixer.RenderMono(
        buffer,
        AudioSettings.SampleRate,
        SilentConfigs(2)
      );

      foreach (var sample in buffer)
        Assert.False(float.IsNaN(sample));
    }

    [Fact]
    public void Render_Stereo_Fully_Writes_Both_Buffers()
    {
      var mixer = new Mixer();
      mixer.Initialize(2, AudioSettings.SampleRate, ChannelMode.Stereo);

      var leftBuffer = new float[AudioSettings.RecommendedBufferSize / 2];
      var rightBuffer = new float[AudioSettings.RecommendedBufferSize / 2];
      Array.Fill(leftBuffer, float.NaN);
      Array.Fill(rightBuffer, float.NaN);

      mixer.RenderStereo(
        leftBuffer,
        rightBuffer,
        AudioSettings.SampleRate,
        SilentConfigs(2)
      );

      foreach (var sample in leftBuffer)
        Assert.False(float.IsNaN(sample));

      foreach (var sample in rightBuffer)
        Assert.False(float.IsNaN(sample));
    }

    [Fact]
    public void Mono_Output_Is_Attenuated()
    {
      var mixer = new Mixer();
      mixer.Initialize(4, AudioSettings.SampleRate);

      var buffer = new float[AudioSettings.RecommendedBufferSize];
      mixer.RenderMono(
        buffer,
        AudioSettings.SampleRate,
        SilentConfigs(4)
      );

      foreach (var sample in buffer)
        Assert.InRange(sample, -1f, 1f);
    }

    [Fact]
    public void Stereo_Output_Is_Attenuated()
    {
      var mixer = new Mixer();
      mixer.Initialize(4, AudioSettings.SampleRate, ChannelMode.Stereo);

      var leftBuffer = new float[AudioSettings.RecommendedBufferSize];
      var rightBuffer = new float[AudioSettings.RecommendedBufferSize];
      mixer.RenderStereo(
        leftBuffer,
        rightBuffer,
        AudioSettings.SampleRate,
        SilentConfigs(4)
      );

      foreach (var sample in leftBuffer)
        Assert.InRange(sample, -1f, 1f);

      foreach (var sample in rightBuffer)
        Assert.InRange(sample, -1f, 1f);
    }

    [Fact]
    public void Mono_Does_Not_Generate_NaNs_Or_Infinities()
    {
      var mixer = new Mixer();
      mixer.Initialize(8, AudioSettings.SampleRate);

      var buffer = new float[AudioSettings.RecommendedBufferSize];

      for (var i = 0; i < 1000; i++)
      {
        mixer.RenderMono(
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
    public void Stereo_Does_Not_Generate_NaNs_Or_Infinities()
    {
      var mixer = new Mixer();
      mixer.Initialize(8, AudioSettings.SampleRate, ChannelMode.Stereo);

      var leftBuffer = new float[AudioSettings.RecommendedBufferSize];
      var rightBuffer = new float[AudioSettings.RecommendedBufferSize];

      for (var i = 0; i < 1000; i++)
      {
        mixer.RenderStereo(
          leftBuffer,
          rightBuffer,
          AudioSettings.SampleRate,
          SilentConfigs(8)
        );

        foreach (var sample in leftBuffer)
        {
          Assert.False(float.IsNaN(sample));
          Assert.False(float.IsInfinity(sample));
        }

        foreach (var sample in rightBuffer)
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
      mixer.RenderMono(
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
      mixer.RenderMono(
        buffer,
        AudioSettings.SampleRate,
        SilentConfigs(2)
      );

      foreach (var sample in buffer)
        Assert.False(float.IsNaN(sample));
    }

    [Fact]
    public void Mono_Is_Deterministic()
    {
      var mixerA = new Mixer();
      var mixerB = new Mixer();
      mixerA.Initialize(3, AudioSettings.SampleRate);
      mixerB.Initialize(3, AudioSettings.SampleRate);

      var bufferA = new float[AudioSettings.RecommendedBufferSize];
      var bufferB = new float[AudioSettings.RecommendedBufferSize];

      for (var i = 0; i < 20; i++)
      {
        mixerA.RenderMono(
          bufferA,
          AudioSettings.SampleRate,
          SilentConfigs(3)
        );
        mixerB.RenderMono(
          bufferB,
          AudioSettings.SampleRate,
          SilentConfigs(3)
        );

        for (var j = 0; j < bufferA.Length; j++)
          Assert.Equal(bufferA[j], bufferB[j], 1e-6f);
      }
    }

    [Fact]
    public void Stereo_Is_Deterministic()
    {
      var mixerA = new Mixer();
      var mixerB = new Mixer();
      mixerA.Initialize(3, AudioSettings.SampleRate, ChannelMode.Stereo);
      mixerB.Initialize(3, AudioSettings.SampleRate, ChannelMode.Stereo);

      var leftBufferA = new float[AudioSettings.RecommendedBufferSize];
      var rightBufferA = new float[AudioSettings.RecommendedBufferSize];
      var leftBufferB = new float[AudioSettings.RecommendedBufferSize];
      var rightBufferB = new float[AudioSettings.RecommendedBufferSize];

      for (var i = 0; i < 20; i++)
      {
        mixerA.RenderStereo(leftBufferA, rightBufferA, AudioSettings.SampleRate, SilentConfigs(3));
        mixerB.RenderStereo(leftBufferB, rightBufferB, AudioSettings.SampleRate, SilentConfigs(3));

        for (var j = 0; j < leftBufferA.Length; j++)
        {
          Assert.Equal(leftBufferA[j], leftBufferB[j], 1e-6f);
          Assert.Equal(rightBufferA[j], rightBufferB[j], 1e-6f);
        }
      }
    }

    private static LayerConfiguration[] SilentConfigs(int count)
    {
      var configs = new LayerConfiguration[count];
      for (int i = 0; i < count; i++)
        configs[i] = default!;

      return configs;
    }
  }
}
