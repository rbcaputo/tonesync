using ToneSync.Core;
using ToneSync.Core.Nodes;

namespace ToneSync.Core.Tests.Nodes
{
  public sealed class EnvelopeTests
  {
    private const float Tolerance = 1e-5f;

    [Fact]
    public void Starts_At_Zero()
    {
      var env = new Envelope();

      Assert.Equal(0f, env.CurrentValue, Tolerance);
    }

    [Fact]
    public void Attack_Moves_Envelope_Upward()
    {
      var env = new Envelope();
      env.Configure(1f, 1f, AudioSettings.SampleRate);
      env.Trigger(true);

      var buffer = new float[AudioSettings.RecommendedBufferSize];
      env.Process(buffer);

      Assert.True(env.CurrentValue > 0f);
      Assert.True(env.CurrentValue < 1f);
    }

    [Fact]
    public void Release_Moves_Envelope_Downward()
    {
      var env = new Envelope();
      env.Configure(0.05f, 0.05f, AudioSettings.SampleRate);
      env.Trigger(true);

      var bufferA = new float[AudioSettings.RecommendedBufferSize];
      for (var i = 0; i < 10; i++)
        env.Process(bufferA);

      var beforeRelease = env.CurrentValue;
      Assert.True(beforeRelease > 0.2f);

      env.Trigger(false);

      var bufferB = new float[AudioSettings.RecommendedBufferSize];
      env.Process(bufferB);

      var afterRelease = env.CurrentValue;
      Assert.True(afterRelease < beforeRelease);
    }

    [Fact]
    public void Envelope_Is_Monotonic_During_Attack()
    {
      var env = new Envelope();
      env.Configure(0.5f, 1f, AudioSettings.SampleRate);
      env.Trigger(true);

      var buffer = new float[AudioSettings.RecommendedBufferSize * 2];
      var last = env.CurrentValue;
      env.Process(buffer);

      foreach (var _ in buffer)
      {
        var current = env.CurrentValue;
        Assert.True(current >= last - Tolerance);

        last = current;
      }
    }

    [Fact]
    public void Envelope_Is_Monotonic_During_Release()
    {
      var env = new Envelope();
      env.Configure(0.1f, 0.5f, AudioSettings.SampleRate);
      env.Trigger(true);

      var bufferA = new float[AudioSettings.RecommendedBufferSize * 4];
      env.Process(bufferA);

      env.Trigger(false);

      var bufferB = new float[AudioSettings.RecommendedBufferSize * 2];
      var last = env.CurrentValue;
      env.Process(bufferB);

      foreach (var _ in bufferB)
      {
        var current = env.CurrentValue;
        Assert.True(current <= last + Tolerance);

        last = current;
      }
    }

    [Fact]
    public void Envelope_Decays_Monotonically_After_Release()
    {
      var envelope = new Envelope();
      envelope.Configure(1f, 1f, 48000);
      envelope.Trigger(true);

      var buffer = new float[AudioSettings.RecommendedBufferSize];
      for (var i = 0; i < 200; i++)
        envelope.Process(buffer);

      envelope.Trigger(false);

      float last = envelope.CurrentValue;

      for (var i = 0; i < 500; i++)
      {
        envelope.Process(buffer);

        Assert.True(envelope.CurrentValue <= last + 1e-7f);
        Assert.True(envelope.CurrentValue >= 0f);

        last = envelope.CurrentValue;
      }
    }

    [Fact]
    public void Attack_Has_Larger_Per_Sample_Delta_Than_Release_When_Configured_So()
    {
      var envelope = new Envelope();
      envelope.Configure(0.1f, 1.0f, AudioSettings.SampleRate);
      envelope.Trigger(true);

      var beforeAttack = envelope.CurrentValue;

      var bufferA = new float[1];
      envelope.Process(bufferA);

      var afterAttack = envelope.CurrentValue;
      var attackDelta = afterAttack - beforeAttack;

      var bufferB = new float[AudioSettings.RecommendedBufferSize];

      // Force envelope near 1
      for (var i = 0; i < 50; i++)
        envelope.Process(bufferB);

      envelope.Trigger(false);

      var beforeRelease = envelope.CurrentValue;

      var bufferC = new float[1];
      envelope.Process(bufferC);

      var afterRelease = envelope.CurrentValue;
      var releaseDelta = beforeRelease - afterRelease;

      Assert.True(attackDelta > releaseDelta);
    }

    [Fact]
    public void Never_Exceeds_Zero_To_One_Range()
    {
      var envelope = new Envelope();
      envelope.Configure(0.1f, 0.1f, AudioSettings.SampleRate);
      envelope.Trigger(true);

      var buffer = new float[AudioSettings.RecommendedBufferSize * 8];
      envelope.Process(buffer);

      Assert.InRange(envelope.CurrentValue, 0f, 1f);

      envelope.Trigger(false);
      envelope.Process(buffer);

      Assert.InRange(envelope.CurrentValue, 0f, 1f);
    }

    [Fact]
    public void Silent_Buffer_Remains_Silent()
    {
      var envelope = new Envelope();
      envelope.Configure(0.1f, 0.1f, AudioSettings.SampleRate);
      envelope.Trigger(true);

      var buffer = new float[AudioSettings.RecommendedBufferSize];
      envelope.Process(buffer);

      foreach (var sample in buffer)
        Assert.Equal(0f, sample, Tolerance);
    }

    [Fact]
    public void Long_Attack_Does_Not_Jump_Abruptly()
    {
      var envelope = new Envelope();
      envelope.Configure(30f, 30f, AudioSettings.SampleRate);
      envelope.Trigger(true);

      var buffer = new float[AudioSettings.RecommendedBufferSize];
      envelope.Process(buffer);

      Assert.True(envelope.CurrentValue < 0.01f);
    }

    [Fact]
    public void Reset_Clears_Envelope_State()
    {
      var envelope = new Envelope();
      envelope.Configure(0.1f, 0.1f, AudioSettings.SampleRate);
      envelope.Trigger(true);

      var bufferA = new float[AudioSettings.RecommendedBufferSize * 2];
      envelope.Process(bufferA);
      Assert.True(envelope.CurrentValue > 0f);

      envelope.Reset();
      Assert.Equal(0f, envelope.CurrentValue, Tolerance);

      var bufferB = new float[AudioSettings.RecommendedBufferSize / 16];
      envelope.Process(bufferB);

      foreach (var sample in bufferB)
        Assert.Equal(0f, sample, Tolerance);
    }
  }
}
