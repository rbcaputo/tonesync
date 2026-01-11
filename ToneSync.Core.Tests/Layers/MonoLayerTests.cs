using ToneSync.Core.Layers;

namespace ToneSync.Core.Tests.Layers
{
  public sealed class MonoLayerTests
  {
    private readonly LayerConfiguration _config = new(440f, 2f, 1f, 1f);

    [Fact]
    public void Active_Layer_Writes_Entire_Buffer()
    {
      var layer = new MonoLayer();
      layer.Initialize(AudioSettings.SampleRate, 0.1f, 0.1f);

      var buffer = new float[AudioSettings.RecommendedBufferSize / 2];
      Array.Fill(buffer, float.NaN);

      layer.UpdateAndProcess(
        buffer,
        AudioSettings.SampleRate,
        _config
      );

      foreach (var sample in buffer)
        Assert.False(float.IsNaN(sample));
    }

    [Fact]
    public void Envelope_Rises_When_Layer_Is_Active()
    {
      var layer = new MonoLayer();
      layer.Initialize(AudioSettings.SampleRate, 0.1f, 0.5f);

      var buffer = new float[AudioSettings.RecommendedBufferSize / 2];

      layer.UpdateAndProcess(
        buffer,
        AudioSettings.SampleRate,
        _config
      );

      Assert.True(layer.CurrentEnvelopeValue > 0f);
    }

    [Fact]
    public void AM_Modulation_Does_Not_Increase_Peak()
    {
      var layer = new MonoLayer();
      layer.Initialize(AudioSettings.SampleRate, 0.1f, 0.1f);

      var bufferNoAM = new float[AudioSettings.RecommendedBufferSize];
      var bufferAM = new float[AudioSettings.RecommendedBufferSize];

      var noAmConfig = new LayerConfiguration(440f, 2f, 0f, 1f);
      var amConfig = new LayerConfiguration(440, 5f, 1f, 1f);

      layer.UpdateAndProcess(bufferNoAM, AudioSettings.SampleRate, noAmConfig);
      layer.Reset();
      layer.Initialize(AudioSettings.SampleRate, 0.1f, 0.1f);
      layer.UpdateAndProcess(bufferAM, AudioSettings.SampleRate, amConfig);

      var peakNoAM = bufferNoAM.Max(MathF.Abs);
      var peakAM = bufferAM.Max(MathF.Abs);
      Assert.True(peakAM <= peakNoAM + 1e-5f);
    }

    [Fact]
    public void Zero_Weight_Generates_Silence()
    {
      var layer = new MonoLayer();
      layer.Initialize(AudioSettings.SampleRate, 0.1f, 0.1f);

      var buffer = new float[AudioSettings.RecommendedBufferSize / 2];

      layer.UpdateAndProcess(
        buffer,
        AudioSettings.SampleRate,
        new LayerConfiguration(440, 2f, 1f, 0f)
      );

      foreach (var sample in buffer)
        Assert.Equal(0f, sample);
    }

    [Fact]
    public void Does_Not_Generate_NaNs()
    {
      var layer = new MonoLayer();
      layer.Initialize(AudioSettings.SampleRate, 0.1f, 0.1f);

      var buffer = new float[AudioSettings.RecommendedBufferSize];

      for (var i = 0; i < 1000; i++)
      {
        layer.UpdateAndProcess(
          buffer,
          AudioSettings.SampleRate,
          _config
        );

        foreach (var sample in buffer)
        {
          Assert.False(float.IsNaN(sample));
          Assert.False(float.IsInfinity(sample));
        }
      }
    }

    [Fact]
    public void Reset_Clears_Envelope_And_Oscillators()
    {
      var layer = new MonoLayer();
      layer.Initialize(AudioSettings.SampleRate, 0.1f, 0.1f);

      var buffer = new float[AudioSettings.RecommendedBufferSize / 2];
      layer.UpdateAndProcess(
        buffer,
        AudioSettings.SampleRate,
        _config
      );

      Assert.True(layer.CurrentEnvelopeValue > 0f);

      layer.Reset();

      Assert.Equal(0f, layer.CurrentEnvelopeValue);
    }

    [Fact]
    public void Is_Deterministic()
    {
      var layerA = new MonoLayer();
      var layerB = new MonoLayer();
      layerA.Initialize(AudioSettings.SampleRate, 0.1f, 0.1f);
      layerB.Initialize(AudioSettings.SampleRate, 0.1f, 0.1f);


      var bufferA = new float[AudioSettings.RecommendedBufferSize];
      var bufferB = new float[AudioSettings.RecommendedBufferSize];

      for (var i = 0; i < 20; i++)
      {
        layerA.UpdateAndProcess(
          bufferA,
          AudioSettings.SampleRate,
          _config
        );
        layerB.UpdateAndProcess(
          bufferB,
          AudioSettings.SampleRate,
          _config
        );

        for (var j = 0; j < bufferA.Length; j++)
          Assert.Equal(bufferA[j], bufferB[j], 1e-6f);
      }
    }
  }
}
