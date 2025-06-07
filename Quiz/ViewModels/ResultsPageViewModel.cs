using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace Quiz.ViewModels
{
    [QueryProperty(nameof(CorrectAnswers), "correct")]
    [QueryProperty(nameof(TotalQuestions), "total")]
    [QueryProperty(nameof(Percentage), "percentage")]
    public class ResultsPageViewModel : INotifyPropertyChanged
    {
        private int _correctAnswers;
        private int _totalQuestions;
        private double _percentage;
        private int _wrongAnswers;
        private string _resultIcon;
        private string _resultTitle;
        private string _performanceMessage;
        private string _progressColor;
        private double _progressValue;

        public int CorrectAnswers
        {
            get => _correctAnswers;
            set
            {
                _correctAnswers = value;
                OnPropertyChanged(nameof(CorrectAnswers));
                UpdateWrongAnswers();
                UpdateResultDisplay();
            }
        }

        public int TotalQuestions
        {
            get => _totalQuestions;
            set
            {
                _totalQuestions = value;
                OnPropertyChanged(nameof(TotalQuestions));
                UpdateWrongAnswers();
                UpdateResultDisplay();
            }
        }

        public double Percentage
        {
            get => _percentage;
            set
            {
                _percentage = value;
                OnPropertyChanged(nameof(Percentage));
                UpdateResultDisplay();
            }
        }

        public int WrongAnswers
        {
            get => _wrongAnswers;
            set
            {
                _wrongAnswers = value;
                OnPropertyChanged(nameof(WrongAnswers));
            }
        }

        public string ResultIcon
        {
            get => _resultIcon;
            set
            {
                _resultIcon = value;
                OnPropertyChanged(nameof(ResultIcon));
            }
        }

        public string ResultTitle
        {
            get => _resultTitle;
            set
            {
                _resultTitle = value;
                OnPropertyChanged(nameof(ResultTitle));
            }
        }

        public string PerformanceMessage
        {
            get => _performanceMessage;
            set
            {
                _performanceMessage = value;
                OnPropertyChanged(nameof(PerformanceMessage));
            }
        }

        public string ProgressColor
        {
            get => _progressColor;
            set
            {
                _progressColor = value;
                OnPropertyChanged(nameof(ProgressColor));
            }
        }

        public double ProgressValue
        {
            get => _progressValue;
            set
            {
                _progressValue = value;
                OnPropertyChanged(nameof(ProgressValue));
            }
        }

        public ICommand PlayAgainCommand { get; }
        public ICommand GoHomeCommand { get; }

        public ResultsPageViewModel()
        {
            PlayAgainCommand = new Command(async () => await PlayAgain());
            GoHomeCommand = new Command(async () => await GoHome());
        }

        private void UpdateWrongAnswers()
        {
            WrongAnswers = TotalQuestions - CorrectAnswers;
        }

        private void UpdateResultDisplay()
        {
            ProgressValue = Percentage / 100.0;

            // Set icon, title, and message based on performance
            if (Percentage >= 90)
            {
                ResultIcon = "🏆";
                ResultTitle = "Excellent!";
                PerformanceMessage = "Outstanding performance! You're a quiz master!";
                ProgressColor = "#4CAF50";
            }
            else if (Percentage >= 70)
            {
                ResultIcon = "🌟";
                ResultTitle = "Great Job!";
                PerformanceMessage = "Well done! You have a good understanding of the topic.";
                ProgressColor = "#8BC34A";
            }
            else if (Percentage >= 50)
            {
                ResultIcon = "👍";
                ResultTitle = "Good Effort!";
                PerformanceMessage = "Not bad! Keep practicing to improve your score.";
                ProgressColor = "#FFC107";
            }
            else
            {
                ResultIcon = "💪";
                ResultTitle = "Keep Trying!";
                PerformanceMessage = "Don't give up! Practice makes perfect.";
                ProgressColor = "#F44336";
            }
        }

        private async Task PlayAgain()
        {
            // Go back to the quiz page (will reload with same category)
            await Shell.Current.GoToAsync("..");
        }

        private async Task GoHome()
        {
            // Navigate back to home page
            await Shell.Current.GoToAsync("//HomePage");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}