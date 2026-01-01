using FreqGen.Core.Nodes;

namespace FreqGen.Core
{
  /// <summary>
  /// Sums multiple audio layers into a single output stream.
  /// Includes safety clamping and headroom management.
  /// </summary>
  public sealed class Mixer
  {
    private readonly List<Layer> _layers = [];
    private readonly float[] _tempBuffer = new float[512];

    /// <summary>
    /// Generates a mixed audio block.
    /// </summary>
    /// <param name="outputBuffer">The buffer to fill with the final mixed signal.</param>
    /// <param name="sampleRate">System audio sample rate.</param>
    /// <param name="configs">The current configurations for all active layers.</param>
    public void Render(Span<float> outputBuffer, float sampleRate, IList<LayerConfiguration> configs)
    {
      outputBuffer.Clear();

      for (int i = 0; i < configs.Count; i++)
      {
        // Sync layer pool
        if (i >= _layers.Count)
          _layers.Add(new());

        Span<float> tempSpam = _tempBuffer.AsSpan(0, outputBuffer.Length);
        tempSpam.Clear();

        _layers[i].UpdateAndProcess(tempSpam, sampleRate, configs[i]);

        // Mix into output
        for (int j = 0; j < outputBuffer.Length; j++)
          outputBuffer[j] += tempSpam[j];
      }

      // Safety clamp
      for (int k = 0; k < outputBuffer.Length; k++)
        outputBuffer[k] = Math.Clamp(outputBuffer[k], -1.0f, 1.0f);
    }

    /// <summary>
    /// Resets the mixer state to prevent clicks on restart.
    /// </summary>
    public void Reset()
    {
      foreach (Layer layer in _layers)
        layer.Reset();
    }
  }
}
