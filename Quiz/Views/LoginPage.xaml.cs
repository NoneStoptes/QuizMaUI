using Microsoft.Maui.Controls;
using Quiz.Models;
using Quiz.Services;
using Quiz.ViewModels;
using System;

namespace Quiz.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        BindingContext = new LoginPageViewModel();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var viewModel = BindingContext as LoginPageViewModel;

        if (viewModel != null)
        { 
            FirebaseServices.Init();
            Person person = await FirebaseServices.AuthenticateUserAsync(viewModel.Email, viewModel.Password);

            if (person != null)
            {
                await DisplayAlert("Login", $"Welcome back, {person.Name}!", "OK");

                // Сохраняем после регистрации или логина
                UserPreferences.SavePerson(person);

                // Переход на главную страницу
                await Navigation.PushAsync(new HomePage());
            }
            else
            {
                await DisplayAlert("Error", "Invalid email or password", "OK");
            }
        }
    }

    private async void OnRegisterTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegistrationPage());
    }
}
