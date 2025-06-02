using Quiz.Models;

namespace Quiz.Views;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
        InitializeComponent();
	}

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        UserPreferences.ClearPerson();
        await Navigation.PushAsync(new LoginPage());
    }

    private async void OnSearchClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Search", $"You searched for: ", "OK");
    }

}