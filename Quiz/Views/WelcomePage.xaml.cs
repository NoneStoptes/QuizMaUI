using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace Quiz.Views
{
    public partial class WelcomePage : ContentPage
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        private async void OnEmailTapped(object sender, EventArgs e)
        {
            // Текст, который нужно скопировать
            var textToCopy = "malv1z.real@gmail.com";

            // Копируем текст в буфер обмена
            await Clipboard.SetTextAsync(textToCopy);

            // Сообщаем об успешном копировании текста
            await DisplayAlert("Copied", "You have been successfully copied the Email!", "OK");
        }

        private async void OnTelegramTapped(object sender, EventArgs e)
        {
            // URL сайта
            var url = "https://t.me/QuizAnnouncement";

            // Проверяем, можно ли открыть ссылку
            if (await Launcher.CanOpenAsync(url))
            {
                // Открываем ссылку
                await Launcher.OpenAsync(url);
            }
            else
            {
                // Обработка ошибки, если URL нельзя открыть
                await DisplayAlert("Ошибка", "Не удалось открыть ссылку.", "OK");
            }
        }

        // Обработчик для кнопки "Login"
        private async void OnLoginClicked(object sender, EventArgs e)
        {
            // Переход на страницу LoginPage
            await Navigation.PushAsync(new LoginPage());
        }

        // Обработчик для кнопки "Register"
        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            // Переход на страницу HomePage
            await Navigation.PushAsync(new RegistrationPage());
        }


        // Обработчик для кнопки "Devloper Only"
        private async void OnDevloperModeClicked(object sender, EventArgs e)
        {
            // Переход на страницу Welcome Page
            await Navigation.PushAsync(new HomePage());
        }
    }
}
