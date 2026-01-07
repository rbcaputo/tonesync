using ToneSync.Core;
using ToneSync.Core.Nodes.Oscillators;

namespace ToneSync.Tests.Core.Nodes.Oscillators
{
  public sealed class SineOscillatorTests
  {
    private const float Tolerance = 1e-5f;

    [Fact]
    public void Outputs_Are_Within_Minus_One_To_One()
    {
      var oscillator = new SineOscillator();
      oscillator.SetFrequency(440f, AudioSettings.SampleRate);

      var buffer = new float[AudioSettings.SampleRate];
      oscillator.Process(buffer);

      foreach (var sample in buffer)
        Assert.InRange(sample, -1f, 1f);
    }

    [Fact]
    public void Generates_Correct_Frequency()
    {
      var oscillator = new SineOscillator();
      oscillator.SetFrequency(440f, AudioSettings.SampleRate);

      var buffer = new float[AudioSettings.RecommendedBufferSize];

      var zeroCrossings = 0;
      var previous = 0f;

      var totalSamples = AudioSettings.SampleRate; // 1 second
      var generated = 0;

      while (generated < totalSamples)
      {
        oscillator.Process(buffer);

        for (var i = 0; i < buffer.Length && generated < totalSamples; generated++, i++)
        {
          var current = buffer[i];

          if (previous <= 0f && current > 0f)
            zeroCrossings++;

          previous = current;
        }
      }

      // 440 Hz ≈ 440 positive-going zero crossings per second
      Assert.InRange(zeroCrossings, 435, 445);
    }

    [Fact]
    public void Phase_Is_Continuous_Across_Buffers()
    {
      var oscillator = new SineOscillator();
      oscillator.SetFrequency(440f, AudioSettings.SampleRate);

      var bufferA = new float[AudioSettings.RecommendedBufferSize / 8];
      var bufferB = new float[AudioSettings.RecommendedBufferSize / 8];

      oscillator.Process(bufferA);
      var lastSample = bufferA[^1];

      oscillator.Process(bufferB);
      var firstSample = bufferB[0];

      Assert.True(Math.Abs(lastSample - firstSample) < 0.1f);
    }

    [Fact]
    public void No_Dc_Offset_Over_Time()
    {
      var oscillator = new SineOscillator();
      oscillator.SetFrequency(440f, AudioSettings.SampleRate);

      var samples = AudioSettings.SampleRate;
      var buffer = new float[samples];
      oscillator.Process(buffer);

      var mean = 0f;
      foreach (var sample in buffer)
        mean += sample;

      mean /= samples;
      Assert.InRange(mean, -1e-4f, 1e-4f);
    }

    [Fact]
    public void Reset_Restarts_Wave_At_Zero_Phase()
    {
      var oscillator = new SineOscillator();
      oscillator.SetFrequency(440f, AudioSettings.SampleRate);

      var buffer = new float[AudioSettings.RecommendedBufferSize / 64];
      oscillator.Process(buffer);

      oscillator.Reset();
      oscillator.Process(buffer);

      Assert.InRange(buffer[0], -Tolerance, Tolerance);
    }

    [Fact]
    public void Is_Deterministic()
    {
      var oscillatorA = new SineOscillator();
      var oscillatorB = new SineOscillator();
      oscillatorA.SetFrequency(440f, AudioSettings.SampleRate);
      oscillatorB.SetFrequency(440f, AudioSettings.SampleRate);

      var bufferA = new float[AudioSettings.RecommendedBufferSize];
      var bufferB = new float[AudioSettings.RecommendedBufferSize];

      for (int i = 0; i < 100; i++)
      {
        oscillatorA.Process(bufferA);
        oscillatorB.Process(bufferB);

        for (int j = 0; j < bufferA.Length; j++)
          Assert.Equal(bufferA[j], bufferB[j], Tolerance);
      }
    }
  }
}