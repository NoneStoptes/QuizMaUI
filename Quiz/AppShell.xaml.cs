// AppShell.xaml.cs
using Quiz.Views;

namespace Quiz;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("HomePage", typeof(HomePage));
        Routing.RegisterRoute("WelcomePage", typeof(WelcomePage));
    }
}