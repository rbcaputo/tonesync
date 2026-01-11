using ToneSync.Core.Layers;

namespace ToneSync.Core.Tests.Layers
{
  public sealed class StereoLayerTests
  {
    private readonly LayerConfiguration _config = new(440f, 2f, 0.5f, 1f, ChannelMode.Stereo, 10f, 0f);

    [Fact]
    public void Inactive_Layer_Outputs_Silence_On_Both_Channels()
    {
      var config = new LayerConfiguration(440f, 2f, 0f, 0f, ChannelMode.Stereo);

      var layer = new StereoLayer();
      layer.Initialize(AudioSettings.SampleRate, 0.1f, 0.1f);

      var leftBuffer = new float[AudioSettings.RecommendedBufferSize / 2];
      var rightBuffer = new float[AudioSettings.RecommendedBufferSize / 2];

      layer.UpdateAndProcess(
        leftBuffer,
        rightBuffer,
        AudioSettings.SampleRate,
        config
      );

      Assert.All(leftBuffer, s => Assert.Equal(0f, s));
      Assert.All(rightBuffer, s => Assert.Equal(0f, s));
    }

    [Fact]
    public void Active_Layer_Writes_Both_Buffers()
    {
      var layer = new StereoLayer();
      layer.Initialize(AudioSettings.SampleRate, 0.1f, 0.1f);

      var leftBuffer = new float[AudioSettings.RecommendedBufferSize / 2];
      var rightBuffer = new float[AudioSettings.RecommendedBufferSize / 2];
      Array.Fill(leftBuffer, float.NaN);
      Array.Fill(rightBuffer, float.NaN);

      layer.UpdateAndProcess(
        leftBuffer,
        rightBuffer,
        AudioSettings.SampleRate,
        _config
      );

      Assert.All(leftBuffer, s => Assert.False(float.IsNaN(s)));
      Assert.All(rightBuffer, s => Assert.False(float.IsNaN(s)));
    }

    [Fact]
    public void Frequency_Offset_Creates_Different_Channels()
    {
      var layer = new StereoLayer();
      layer.Initialize(AudioSettings.SampleRate, 0.1f, 0.1f);

      var leftBuffer = new float[AudioSettings.RecommendedBufferSize];
      var rightBuffer = new float[AudioSettings.RecommendedBufferSize];

      layer.UpdateAndProcess(
        leftBuffer,
        rightBuffer,
        AudioSettings.SampleRate,
        _config
      );

      bool isDifferent = false;
      for (var i = 0; i < leftBuffer.Length; i++)
        if (Math.Abs(leftBuffer[i] - rightBuffer[i]) > 1e-6f)
        {
          isDifferent = true;
          break;
        }

      Assert.True(isDifferent, "Channels should differ with frequency offset");
    }

    [Fact]
    public void Zero_Offset_Creates_Identical_Channels()
    {
      var layer = new StereoLayer();
      layer.Initialize(AudioSettings.SampleRate, 0.1f, 0.1f);

      var leftBuffer = new float[AudioSettings.RecommendedBufferSize / 2];
      var rightBuffer = new float[AudioSettings.RecommendedBufferSize / 2];

      var config = new LayerConfiguration(440f, 2f, 0.5f, 1f, ChannelMode.Stereo);

      layer.UpdateAndProcess(
        leftBuffer,
        rightBuffer,
        AudioSettings.SampleRate,
        config
      );

      for (var i = 0; i < leftBuffer.Length; i++)
        Assert.Equal(leftBuffer[i], rightBuffer[i], 1e-6f);
    }

    [Fact]
    public void Does_Not_Generate_NaNs()
    {
      var layer = new StereoLayer();
      layer.Initialize(AudioSettings.SampleRate, 0.1f, 0.1f);

      var leftBuffer = new float[AudioSettings.RecommendedBufferSize];
      var rightBuffer = new float[AudioSettings.RecommendedBufferSize];

      for (var i = 0; i < 1000; i++)
      {
        layer.UpdateAndProcess(
          leftBuffer,
          rightBuffer,
          AudioSettings.SampleRate,
          _config
        );

        Assert.All(leftBuffer, s =>
        {
          Assert.False(float.IsNaN(s));
          Assert.False(float.IsInfinity(s));
        });
        Assert.All(rightBuffer, s =>
        {
          Assert.False(float.IsNaN(s));
          Assert.False(float.IsInfinity(s));
        });
      }
    }

    [Fact]
    public void Reset_Clears_Both_Channels()
    {
      var layer = new StereoLayer();
      layer.Initialize(AudioSettings.SampleRate, 0.1f, 0.1f);

      var leftBuffer = new float[AudioSettings.RecommendedBufferSize / 2];
      var rightBuffer = new float[AudioSettings.RecommendedBufferSize / 2];

      layer.UpdateAndProcess(
        leftBuffer,
        rightBuffer,
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
      var layerA = new StereoLayer();
      var layerB = new StereoLayer();
      layerA.Initialize(AudioSettings.SampleRate, 0.1f, 0.1f);
      layerB.Initialize(AudioSettings.SampleRate, 0.1f, 0.1f);

      var leftBufferA = new float[AudioSettings.RecommendedBufferSize];
      var rightBufferA = new float[AudioSettings.RecommendedBufferSize];
      var leftBufferB = new float[AudioSettings.RecommendedBufferSize];
      var rightBufferB = new float[AudioSettings.RecommendedBufferSize];

      for (var i = 0; i < 20; i++)
      {
        layerA.UpdateAndProcess(
          leftBufferA,
          rightBufferA,
          AudioSettings.SampleRate,
          _config
        );
        layerB.UpdateAndProcess(
          leftBufferB,
          rightBufferB,
          AudioSettings.SampleRate,
          _config
        );

        for (var j = 0; j < leftBufferA.Length; j++)
        {
          Assert.Equal(leftBufferA[j], leftBufferB[j], 1e-6f);
          Assert.Equal(rightBufferA[j], rightBufferB[j], 1e-6f);
        }
      }
    }
  }
}
