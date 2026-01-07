using ToneSync.Core;
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
    public void Fill_Buffer_Not_Initialized_Clears_Buffer()
    {
      var engine = new AudioEngine();
      var buffer = new float[AudioSettings.RecommendedBufferSize / 8];

      buffer[0] = 1f;
      engine.FillBuffer(buffer);

      Assert.All(buffer, s => Assert.Equal(0f, s));
    }

    [Fact]
    public void Fill_Buffer_Not_Playing_Clears_Buffer()
    {
      var engine = new AudioEngine(AudioSettings.SampleRate);
      engine.Initialize([ActiveConfig()]);

      var buffer = new float[AudioSettings.RecommendedBufferSize / 8];

      buffer[0] = 1f;
      engine.FillBuffer(buffer);

      Assert.All(buffer, s => Assert.Equal(0f, s));
    }

    [Fact]
    public void Fill_Buffer_When_Playing_Generates_Non_Zero_Signal()
    {
      var engine = new AudioEngine(AudioSettings.SampleRate);
      engine.Initialize([ActiveConfig()]);
      engine.Start();

      var buffer = new float[AudioSettings.RecommendedBufferSize / 4];
      engine.FillBuffer(buffer);

      Assert.True(MaxAbs(buffer) > 1e-6f);
    }

    [Fact]
    public void Fill_Buffer_Always_Clamps_Output()
    {
      var engine = new AudioEngine(AudioSettings.SampleRate);
      engine.Initialize([ActiveConfig()]);
      engine.Start();
      engine.SetMasterGain(1f);
      engine.OutputGain = 1f;

      var buffer = new float[AudioSettings.RecommendedBufferSize];

      for (var i = 0; i < 10; i++)
        engine.FillBuffer(buffer);

      Assert.All(buffer, s => Assert.InRange(s, -0.999f, 0.999f));
    }

    [Fact]
    public void Output_Gain_Scales_Signal()
    {
      static float RenderWithGain(float gain)
      {
        var engine = new AudioEngine(AudioSettings.SampleRate);
        engine.Initialize([ActiveConfig()]);
        engine.SetMasterGain(1.0f); // neutralize master gain
        engine.OutputGain = gain;
        engine.Start();

        var buffer = new float[AudioSettings.RecommendedBufferSize / 4];
        engine.FillBuffer(buffer);

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
      engine.FillBuffer(bufferA);

      engine.SetMasterGain(1f);
      engine.FillBuffer(bufferB);

      // Should not jump instantly to full amplitude
      Assert.True(MaxAbs(bufferB) < 0.9f);
    }

    [Fact]
    public void Stop_Silences_Subsequent_Buffers()
    {
      var engine = new AudioEngine(AudioSettings.SampleRate);
      engine.Initialize([ActiveConfig()]);
      engine.Start();

      var buffer = new float[AudioSettings.RecommendedBufferSize / 4];
      engine.FillBuffer(buffer);

      engine.Stop();
      engine.FillBuffer(buffer);

      Assert.All(buffer, s => Assert.Equal(0f, s));
    }

    [Fact]
    public void Reset_Silences_Engine()
    {
      var engine = new AudioEngine(AudioSettings.SampleRate);
      engine.Initialize([ActiveConfig()]);
      engine.Start();

      var buffer = new float[AudioSettings.RecommendedBufferSize / 4];
      engine.FillBuffer(buffer);

      engine.Reset();
      engine.FillBuffer(buffer);

      Assert.All(buffer, s => Assert.Equal(0f, s));
    }


    [Fact]
    public void Dispose_Prevents_Further_Use()
    {
      var engine = new AudioEngine(AudioSettings.SampleRate);
      engine.Initialize([ActiveConfig()]);
      engine.Dispose();

      Assert.Throws<ObjectDisposedException>(() =>
        engine.FillBuffer(new float[AudioSettings.RecommendedBufferSize / 8])
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
      Weight = weight
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
