using FreqGen.App.ViewModels;

namespace FreqGen.App.Views
{
  public sealed partial class MainPage : ContentPage
  {
    private readonly MainViewModel _viewModel;

    public MainPage(MainViewModel viewModel)
    {
      InitializeComponent();

      _viewModel = viewModel;
      BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
      base.OnAppearing();
      await _viewModel.InitializeAsync();
    }

    protected override void OnDisappearing()
    {
      base.OnDisappearing();

      // Ensure audio stops when page is not visible
      if (_viewModel.IsPlaying)
        _viewModel.StopCommand.ExecuteAsync(null);
    }
  }
}
