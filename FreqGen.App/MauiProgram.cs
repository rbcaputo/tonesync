using FreqGen.App.Services;

namespace FreqGen.App
{
  public static class MauiProgram
  {
    public static MauiApp CreateMauiApp()
    {
      MauiAppBuilder builder = MauiApp.CreateBuilder();

      builder
        .UseMauiApp<App>()
        .ConfigureFonts(fonts =>
        {
          fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
          fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        });

      // Register services
      builder.Services.AddSingleton<IAudioService, AudioService>();

      // Register ViewModels
      builder.Services.AddTransient<MainViewModel>();

      // Register Views
      builder.Services.AddTransient<MainPage>();

#if DEBUG

      builder.Logging.AddDebug();

#endif

      return builder.Build();
    }
  }
}