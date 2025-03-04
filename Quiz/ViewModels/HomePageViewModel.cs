using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Quiz.ViewModels
{
    public class Card
    {
        public string Title { get; set; }
        public string Image { get; set; }
    }

    public class HomePageViewModel : INotifyPropertyChanged
    {
        private string _selectedLevel;
        private bool _areCardsVisible;

        public string SelectedLevel
        {
            get => _selectedLevel;
            set
            {
                if (_selectedLevel != value)
                {
                    _selectedLevel = value;
                    OnPropertyChanged(nameof(SelectedLevel));

                    // Обновляем видимость карточек
                    UpdateCards();
                }
            }
        }

        public bool AreCardsVisible
        {
            get => _areCardsVisible;
            set
            {
                if (_areCardsVisible != value)
                {
                    _areCardsVisible = value;
                    OnPropertyChanged(nameof(AreCardsVisible));
                }
            }
        }

        public ObservableCollection<Card> Cards { get; set; } = new ObservableCollection<Card>();

        private void UpdateCards()
        {
            Cards.Clear();

            if (SelectedLevel == "Very Easy")
            {
                Cards.Add(new Card { Title = "Animals", Image = "cat.png" });
                Cards.Add(new Card { Title = "Food", Image = "salad.png" });
                Cards.Add(new Card { Title = "Sports", Image = "football.png" });
                Cards.Add(new Card { Title = "Geography", Image = "globe.png" });
                AreCardsVisible = true;
            }
            else if (SelectedLevel == "Easy")
            {
                Cards.Add(new Card { Title = "Food", Image = "salad.png" });
                AreCardsVisible = true;
            }
            else if (SelectedLevel == "Medium")
            {
                Cards.Add(new Card { Title = "Sports", Image = "football.png" });
                AreCardsVisible = true;
            }
            else if (SelectedLevel == "Hard")
            {
                Cards.Add(new Card { Title = "Geography", Image = "globe.png" });
                AreCardsVisible = true;
            }
            else
            {
                AreCardsVisible = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
