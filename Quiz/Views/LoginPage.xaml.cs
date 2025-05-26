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

            // 1. Получаем пользователя по email
            var allPersons = await FirebaseServices.GetAllPersonsAsync(); // Новый метод ниже
            var person = allPersons.FirstOrDefault(p => p.Email == viewModel.Email);

            if (person == null)
            {
                await DisplayAlert("Error", "User not found", "OK");
                return;
            }

            // 2. Получаем пепер по индексу
            var (pepper, pepperIndex) = await FirebaseServices.GetPepperByIndex_FromArray(person.PepperIndex);
            var hasher = new PasswordHasher(pepper);

            // 3. Проверка пароля
            bool isValid = hasher.Verify(viewModel.Password, person.Password);

            if (isValid)
            {
                await DisplayAlert("Login", $"Welcome back, {person.Name}!", "OK");
                UserPreferences.SavePerson(person);
                await Navigation.PushAsync(new HomePage());
            }
            else
            {
                await DisplayAlert("Error", "Invalid password", "OK");
            }
        }
    }

    private async void OnRegisterTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegistrationPage());
    }
}
