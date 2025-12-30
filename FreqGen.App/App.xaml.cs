namespace FreqGen.App
{
  public sealed partial class App : Application
  {
    public App()
    {
      InitializeComponent();
      MainPage = new NavigationPage(new MainPage());
    }
  }
}