using Quiz.Models;
using Quiz.ViewModels;

namespace Quiz.Views;

public partial class HomePage : ContentPage
{
    private HomePageViewModel _viewModel;

    public HomePage()
    {
        InitializeComponent();
        _viewModel = (HomePageViewModel)BindingContext;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        // Update popup size when window size changes
        if (_viewModel != null && width > 0 && height > 0)
        {
            _viewModel.PopupWidth = width * 0.5;
            _viewModel.PopupHeight = height * 0.5;
        }
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        UserPreferences.ClearPerson();
        await Shell.Current.GoToAsync("//WelcomePage");
    }

    private async void OnSearchClicked(object sender, EventArgs e)
    {
        var searchText = SearchEntry.Text;
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            await _viewModel.SearchCategoriesAsync(searchText);
        }
    }

    private void OnDifficultyChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value && sender is RadioButton radioButton)
        {
            var selectedDifficulty = radioButton.Value?.ToString();
            if (!string.IsNullOrEmpty(selectedDifficulty))
            {
                _viewModel.SelectedDifficulty = selectedDifficulty;
            }
        }
    }
}