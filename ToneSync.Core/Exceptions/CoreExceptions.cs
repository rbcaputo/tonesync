namespace ToneSync.Core.Exceptions
{
  /// <summary>
  /// Base exception for all FreqGen.Core errors.
  /// </summary>
  public class CoreException(string message, Exception? inner = null)
    : Exception(message, inner);

  /// <summary>
  /// Thrown when the audio hardware fails to initialize or a buffer cannot be filled.
  /// </summary>
  public sealed class AudioPlaybackException(string message, Exception? inner = null)
    : CoreException(message, inner);

  /// <summary>
  /// Thrown when a LayerConfiguration contains values outside of the allowed AudioSettings.
  /// </summary>
  public sealed class InvalidConfigurationException(string message, string paramName)
    : CoreException($"{message} Parameter: {paramName}");
}
