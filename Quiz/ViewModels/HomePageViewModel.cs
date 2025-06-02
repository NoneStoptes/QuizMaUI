using System.ComponentModel;
using Quiz.Models;         // Тут должны быть классы Person, UserPreferences, и т.д.
using Quiz;                // если UserPreferences находится в корневом пространстве имён Quiz

namespace Quiz.ViewModels
{
    public class HomePageViewModel : INotifyPropertyChanged
    {
        private string _greeting;
        public string Greeting
        {
            get => _greeting;
            set
            {
                if (_greeting != value)
                {
                    _greeting = value;
                    OnPropertyChanged(nameof(Greeting));
                }
            }
        }

        public HomePageViewModel()
        {
            // Загрузка текущего пользователя
            var person = UserPreferences.LoadPerson();

            if (person != null && !string.IsNullOrEmpty(person.Name))
                Greeting = $"Welcome Back {person.Name}";
            else
                Greeting = "Welcome Back"; // Или оставить пустым: string.Empty
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
