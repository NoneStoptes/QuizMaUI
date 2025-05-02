using Microsoft.Maui.Controls;
using Quiz.Models;
using Quiz.Services;
using Quiz.ViewModels;

namespace Quiz.Views;

public partial class RegistrationPage : ContentPage
{
    public RegistrationPage()
    {
        InitializeComponent();
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        FirebaseServices.Init();

        var viewModel = BindingContext as ViewModels.RegistrationPageViewModel;

        if (viewModel != null)
        {
            var person = new Person
            {
                Name = viewModel.Name,
                Nickname = viewModel.Nickname,
                Email = viewModel.Email,
                Password = viewModel.Password,
            };

            System.Diagnostics.Debug.WriteLine($"NickName is : {person.Nickname}\nEmail is : {person.Email}");

            var service = new FirebaseServices();

            // 🔹 Проверка существования в базе
            bool isNicknameExists = await service.IsNicknameExists(person.Nickname);
            bool isEmailExists = await service.IsEmailExists(person.Email);
            System.Diagnostics.Debug.WriteLine($"Nickname exists: {isNicknameExists}, Email exists: {isEmailExists}");


            if(isNicknameExists || isEmailExists)
            {
                // Устанавливаем ошибки
                viewModel.NicknameError = isNicknameExists ? "This Nickname is already registered" : ""; // Syntex error
                viewModel.IsNicknameErrorVisible = isNicknameExists; // Syntex error

                viewModel.EmailError = isEmailExists ? "This Email is already registered" : ""; // Syntex error
                viewModel.IsEmailErrorVisible = isEmailExists; // Syntex error

            }
            else
            {
                // ✅ Если ошибок нет — продолжаем регистрацию
                await FirebaseServices.AddUserAsync(person); // await чтобы дождаться завершения

                await DisplayAlert("Registration", "You have been successfully registered!", "OK");

                await Navigation.PushAsync(new LoginPage());

            }
        }
    }

    private async void OnLoginTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LoginPage());
    }
}