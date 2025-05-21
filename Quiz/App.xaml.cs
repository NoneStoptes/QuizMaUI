// App.xaml.cs
using Quiz.Models;
using Microsoft.Maui.Controls;
using Quiz.Services;

namespace Quiz;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        FirebaseServices.Init();        // если нужна инициализация

        MainPage = new AppShell();      // назначаем Shell

        // После того как Shell готов — проверяем куки
        Application.Current
            .Dispatcher
            .Dispatch(async () =>
            {
                if (UserPreferences.IsUserLoggedIn())
                {
                    // если есть сохранённые данные — на HomePage
                    await Shell.Current.GoToAsync("//HomePage");
                }
                else
                {
                    // если нет — на WelcomePage
                    await Shell.Current.GoToAsync("//WelcomePage");
                }
            });
    }
}
