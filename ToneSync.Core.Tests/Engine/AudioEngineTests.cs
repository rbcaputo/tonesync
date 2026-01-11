using ToneSync.Core.Engine;
using ToneSync.Core.Exceptions;
using ToneSync.Core.Layers;

namespace ToneSync.Core.Tests.Engine
{
  public sealed class AudioEngineTests
  {
    [Fact]
    public void Constructor_Invalid_Sample_Rate_Throws()
    {
      Assert.Throws<ArgumentException>(() =>
        new AudioEngine(1000f)
      );
      Assert.Throws<ArgumentException>(() =>
        new AudioEngine(500_000f)
      );
    }

    [Fact]
    public void Start_Before_Initialize_Throws()
    {
      var engine = new AudioEngine(AudioSettings.SampleRate);
      Assert.Throws<InvalidOperationException>(engine.Start);
    }

    [Fact]
    public void Initialize_With_Empty_Configs_Throws()
    {
      var engine = new AudioEngine();
      Assert.Throws<InvalidConfigurationException>(() =>
        engine.Initialize([])
      );
    }

    [Fact]
    public void Initialize_Sets_Is_Initialized()
    {
      var engine = new AudioEngine(AudioSettings.SampleRate);
      engine.Initialize([ActiveConfig()]);
      Assert.True(engine.IsInitialized);
    }

    [Fact]
    public void Initialize_Defaults_To_Mono_Mode()
    {
      var engine = new AudioEngine(AudioSettings.SampleRate);
      engine.Initialize([ActiveConfig()]);
      Assert.Equal(ChannelMode.Mono, engine.ChannelMode);
    }

    [Fact]
    public void Initialize_With_Stereo_Mode_Sets_Channel_Mode()
    {
      var engine = new AudioEngine(AudioSettings.SampleRate);
      engine.Initialize([ActiveConfig()], ChannelMode.Stereo);
      Assert.Equal(ChannelMode.Stereo, engine.ChannelMode);
    }

    [Fact]
    public void Fill_Buffer_Not_Initialized_Clears_Buffer()
    {
      var engine = new AudioEngine();
      var buffer = new float[AudioSettings.RecommendedBufferSize / 8];

      buffer[0] = 1f;
      engine.FillMonoBuffer(buffer);

      Assert.All(buffer, s => Assert.Equal(0f, s));
    }

    [Fact]
    public void Fill_Buffer_Not_Playing_Clears_Buffer()
    {
      var engine = new AudioEngine(AudioSettings.SampleRate);
      engine.Initialize([ActiveConfig()]);

      var buffer = new float[AudioSettings.RecommendedBufferSize / 8];

      buffer[0] = 1f;
      engine.FillMonoBuffer(buffer);

      Assert.All(buffer, s => Assert.Equal(0f, s));
    }

    [Fact]
    public void Fill_Mono_Buffer_When_Playing_Generates_Non_Zero_Signal()
    {
      var engine = new AudioEngine(AudioSettings.SampleRate);
      engine.Initialize([ActiveConfig()]);
      engine.Start();

      var buffer = new float[AudioSettings.RecommendedBufferSize / 4];
      engine.FillMonoBuffer(buffer);

      Assert.True(MaxAbs(buffer) > 1e-6f);
    }

    [Fact]
    public void Fill_Stereo_Buffer_When_Playing_Generates_Non_Zero_Signal()
    {
      var engine = new AudioEngine(AudioSettings.SampleRate);
      engine.Initialize([ActiveConfig()], ChannelMode.Stereo);
      engine.Start();

      var leftBuffer = new float[AudioSettings.RecommendedBufferSize / 4];
      var rightBuffer = new float[AudioSettings.RecommendedBufferSize / 4];
      engine.FillStereoBuffer(leftBuffer, rightBuffer);

      Assert.True(MaxAbs(leftBuffer) > 1e-6f);
      Assert.True(MaxAbs(rightBuffer) > 1e-6f);
    }

    [Fact]
    public void Fill_Mono_Buffer_With_Stereo_Engine_Throes()
    {
      var engine = new AudioEngine(AudioSettings.SampleRate);
      engine.Initialize([ActiveConfig()], ChannelMode.Stereo);
      engine.Start();

      var buffer = new float[AudioSettings.RecommendedBufferSize / 4];

      Assert.Throws<InvalidOperationException>(() =>
        engine.FillMonoBuffer(buffer)
      );
    }

    [Fact]
    public void Fill_Stereo_Buffer_With_Mono_Engine_Throws()
    {
      var engine = new AudioEngine(AudioSettings.SampleRate);
      engine.Initialize([ActiveConfig()], ChannelMode.Mono);
      engine.Start();

      var leftBuffer = new float[AudioSettings.RecommendedBufferSize / 4];
      var rightBuffer = new float[AudioSettings.RecommendedBufferSize / 4];

      Assert.Throws<InvalidOperationException>(() =>
        engine.FillStereoBuffer(leftBuffer, rightBuffer)
      );
    }

    [Fact]
    public void Fill_Stereo_Buffer_With_Mismatched_Buffer_Sizes_Throws()
    {
      var engine = new AudioEngine(AudioSettings.SampleRate);
      engine.Initialize([ActiveConfig()], ChannelMode.Stereo);
      engine.Start();

      var leftBuffer = new float[AudioSettings.RecommendedBufferSize / 4];
      var rightBuffer = new float[AudioSettings.RecommendedBufferSize / 2];

      Assert.Throws<ArgumentException>(() =>
        engine.FillStereoBuffer(leftBuffer, rightBuffer)
      );
    }

    [Fact]
    public void Fill_Stereo_Buffer_Not_Playing_Clears_Both_Buffers()
    {
      var engine = new AudioEngine(AudioSettings.SampleRate);
      engine.Initialize([ActiveConfig()], ChannelMode.Stereo);

      var leftBuffer = new float[AudioSettings.RecommendedBufferSize / 8];
      var rightBuffer = new float[AudioSettings.RecommendedBufferSize / 8];
      leftBuffer[0] = 1f;
      rightBuffer[0] = 1f;

      engine.FillStereoBuffer(leftBuffer, rightBuffer);

      Assert.All(leftBuffer, s => Assert.Equal(0f, s));
      Assert.All(rightBuffer, s => Assert.Equal(0f, s));
    }

    [Fact]
    public void Mono_Layer_In_Stereo_Mode_Produces_Identical_Channels()
    {
      var config = ActiveConfig() with
      {
        ChannelMode = ChannelMode.Mono,
        Pan = 0f
      };

      var engine = new AudioEngine(AudioSettings.SampleRate);
      engine.Initialize([config], ChannelMode.Stereo);
      engine.Start();

      var leftBuffer = new float[AudioSettings.RecommendedBufferSize / 4];
      var rightBuffer = new float[AudioSettings.RecommendedBufferSize / 4];
      engine.FillStereoBuffer(leftBuffer, rightBuffer);

      for (var i = 0; i < leftBuffer.Length; i++)
        Assert.Equal(leftBuffer[i], rightBuffer[i], 1e-5f);
    }

    [Fact]
    public void Mono_Layer_Panned_Left_Attenuates_Right_Channel()
    {
      var config = ActiveConfig() with
      {
        ChannelMode = ChannelMode.Mono,
        Pan = -1f
      };

      var engine = new AudioEngine(AudioSettings.SampleRate);
      engine.Initialize([config], ChannelMode.Mono);
      engine.Start();

      var leftBuffer = new float[AudioSettings.RecommendedBufferSize / 4];
      var rightBuffer = new float[AudioSettings.RecommendedBufferSize / 4];
      engine.FillStereoBuffer(leftBuffer, rightBuffer);

      var leftPeak = MaxAbs(leftBuffer);
      var rightPeak = MaxAbs(rightBuffer);

      Assert.True(leftPeak > rightPeak * 10f, "Left channel should be much louder when panned full left");
    }

    [Fact]
    public void Mono_Layer_Panned_Right_Attenuates_Left_Channel()
    {
      var config = ActiveConfig() with
      {
        ChannelMode = ChannelMode.Mono,
        Pan = 1f
      };

      var engine = new AudioEngine(AudioSettings.SampleRate);
      engine.Initialize([config], ChannelMode.Stereo);
      engine.Start();

      var leftBuffer = new float[AudioSettings.RecommendedBufferSize / 4];
      var rightBuffer = new float[AudioSettings.RecommendedBufferSize / 4];
      engine.FillStereoBuffer(leftBuffer, rightBuffer);

      float leftPeak = MaxAbs(leftBuffer);
      float rightPeak = MaxAbs(rightBuffer);

      Assert.True(rightPeak > leftPeak * 10f, "Right channel should be much louder when panned full right");
    }

    [Fact]
    public void Stereo_Layer_With_Frequency_Offset_Produces_Different_Channels()
    {
      var config = ActiveConfig() with
      {
        ChannelMode = ChannelMode.Stereo,
        StereoFrequencyOffset = 10f
      };

      var engine = new AudioEngine(AudioSettings.SampleRate);
      engine.Initialize([config], ChannelMode.Stereo);
      engine.Start();

      var leftBuffer = new float[AudioSettings.RecommendedBufferSize];
      var rightBuffer = new float[AudioSettings.RecommendedBufferSize];
      engine.FillStereoBuffer(leftBuffer, rightBuffer);

      bool isDifferent = false;
      for (var i = 0; i < leftBuffer.Length; i++)
        if (Math.Abs(leftBuffer[i] - rightBuffer[i]) > 1e-6f)
        {
          isDifferent = true;
          break;
        }

      Assert.True(isDifferent, "Left and right channels should differ with frequency offset");
    }

    [Fact]
    public void Fill_Mono_Buffer_Always_Clamps_Output()
    {
      var engine = new AudioEngine(AudioSettings.SampleRate);
      engine.Initialize([ActiveConfig()]);
      engine.Start();
      engine.SetMasterGain(1f);
      engine.OutputGain = 1f;

      var buffer = new float[AudioSettings.RecommendedBufferSize];

      for (var i = 0; i < 10; i++)
        engine.FillMonoBuffer(buffer);

      Assert.All(buffer, s => Assert.InRange(s, -0.999f, 0.999f));
    }

    [Fact]
    public void Fill_Stereo_Buffer_Always_Clamps_Output()
    {
      var engine = new AudioEngine(AudioSettings.SampleRate);
      engine.Initialize([ActiveConfig()], ChannelMode.Stereo);
      engine.Start();
      engine.SetMasterGain(1f);
      engine.OutputGain = 1f;

      var leftBuffer = new float[AudioSettings.RecommendedBufferSize];
      var rightBuffer = new float[AudioSettings.RecommendedBufferSize];

      for (var i = 0; i < 10; i++)
        engine.FillStereoBuffer(leftBuffer, rightBuffer);

      Assert.All(leftBuffer, s => Assert.InRange(s, -0.999f, 0.999f));
      Assert.All(rightBuffer, s => Assert.InRange(s, -0.999f, 0.999f));
    }

    [Fact]
    public void Output_Gain_Scales_Signal()
    {
      static float RenderWithGain(float gain)
      {
        var engine = new AudioEngine(AudioSettings.SampleRate);
        engine.Initialize([ActiveConfig()]);
        engine.SetMasterGain(1.0f);
        engine.OutputGain = gain;
        engine.Start();

        var buffer = new float[AudioSettings.RecommendedBufferSize / 4];
        engine.FillMonoBuffer(buffer);

        engine.Dispose();
        return MaxAbs(buffer);
      }

      var peakFull = RenderWithGain(1.0f);
      var peakHalf = RenderWithGain(0.5f);
      Assert.True(peakFull > 0f);
      Assert.InRange(peakHalf / peakFull, 0.49f, 0.51f);
    }

    [Fact]
    public void Master_Gain_Is_Smoothed_Not_Instant()
    {
      var engine = new AudioEngine(AudioSettings.SampleRate);
      engine.Initialize([ActiveConfig()]);
      engine.Start();

      var bufferA = new float[AudioSettings.RecommendedBufferSize / 4];
      var bufferB = new float[AudioSettings.RecommendedBufferSize / 4];

      engine.SetMasterGain(0f);
      engine.FillMonoBuffer(bufferA);

      engine.SetMasterGain(1f);
      engine.FillMonoBuffer(bufferB);

      // Should not jump instantly to full amplitude
      Assert.True(MaxAbs(bufferB) < 0.9f);
    }

    [Fact]
    public void Stop_Silences_Subsequent_Mono_Buffers()
    {
      var engine = new AudioEngine(AudioSettings.SampleRate);
      engine.Initialize([ActiveConfig()]);
      engine.Start();

      var buffer = new float[AudioSettings.RecommendedBufferSize / 4];
      engine.FillMonoBuffer(buffer);

      engine.Stop();
      engine.FillMonoBuffer(buffer);

      Assert.All(buffer, s => Assert.Equal(0f, s));
    }

    [Fact]
    public void Stop_Silences_Subsequent_Stereo_Buffers()
    {
      var engine = new AudioEngine(AudioSettings.SampleRate);
      engine.Initialize([ActiveConfig()], ChannelMode.Stereo);
      engine.Start();

      var leftBuffer = new float[AudioSettings.RecommendedBufferSize / 4];
      var rightBuffer = new float[AudioSettings.RecommendedBufferSize / 4];
      engine.FillStereoBuffer(leftBuffer, rightBuffer);

      engine.Stop();
      engine.FillStereoBuffer(leftBuffer, rightBuffer);

      Assert.All(leftBuffer, s => Assert.Equal(0f, s));
      Assert.All(rightBuffer, s => Assert.Equal(0f, s));
    }

    [Fact]
    public void Reset_Silences_Engine()
    {
      var engine = new AudioEngine(AudioSettings.SampleRate);
      engine.Initialize([ActiveConfig()]);
      engine.Start();

      var buffer = new float[AudioSettings.RecommendedBufferSize / 4];
      engine.FillMonoBuffer(buffer);

      engine.Reset();
      engine.FillMonoBuffer(buffer);

      Assert.All(buffer, s => Assert.Equal(0f, s));
    }

    [Fact]
    public void Dispose_Prevents_Further_Mono_Use()
    {
      var engine = new AudioEngine(AudioSettings.SampleRate);
      engine.Initialize([ActiveConfig()]);
      engine.Dispose();

      Assert.Throws<ObjectDisposedException>(() =>
        engine.FillMonoBuffer(new float[AudioSettings.RecommendedBufferSize / 8])
      );
    }

    [Fact]
    public void Dispose_Prevents_Further_Stereo_Use()
    {
      var engine = new AudioEngine(AudioSettings.SampleRate);
      engine.Initialize([ActiveConfig()], ChannelMode.Stereo);
      engine.Dispose();

      var leftBuffer = new float[AudioSettings.RecommendedBufferSize / 4];
      var rightBuffer = new float[AudioSettings.RecommendedBufferSize / 4];

      Assert.Throws<ObjectDisposedException>(() =>
          engine.FillStereoBuffer(leftBuffer, rightBuffer)
      );
    }

    private static LayerConfiguration ActiveConfig(
      float carrHz = 440f,
      float modHz = 2f,
      float modDepth = 1f,
      float weight = 1f
    ) => new()
    {
      IsActive = true,
      CarrierFrequency = carrHz,
      ModulatorFrequency = modHz,
      ModulatorDepth = modDepth,
      Weight = weight,
      ChannelMode = ChannelMode.Mono,
      StereoFrequencyOffset = 0f,
      Pan = 0f
    };

    private static float MaxAbs(float[] buffer)
    {
      float max = 0f;

      foreach (float sample in buffer)
        max = Math.Max(max, Math.Abs(sample));

      return max;
    }
  }
}
