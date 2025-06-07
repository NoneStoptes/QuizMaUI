using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Quiz.Models;
using Quiz.Services;

namespace Quiz.ViewModels
{
    [QueryProperty(nameof(CategoryId), "categoryId")]
    [QueryProperty(nameof(TotalQuestions), "totalQuestions")]
    public class QuizPageViewModel : INotifyPropertyChanged
    {
        private string _categoryId;
        private int _totalQuestions;
        private bool _isPreGame = true;
        private bool _isPlaying = false;
        private bool _showFeedback = false;
        private QuizQuestion _currentQuestion;
        private List<QuizQuestion> _questions;
        private int _currentQuestionIndex = 0;
        private int _correctAnswers = 0;
        private double _progress = 0;
        private string _progressText;
        private string _questionCounterText;
        private string _feedbackText;
        private string _feedbackIcon;
        private string _feedbackColor;

        public string CategoryId
        {
            get => _categoryId;
            set
            {
                _categoryId = value;
                OnPropertyChanged(nameof(CategoryId));
            }
        }

        public int TotalQuestions
        {
            get => _totalQuestions;
            set
            {
                _totalQuestions = value;
                OnPropertyChanged(nameof(TotalQuestions));
            }
        }

        public bool IsPreGame
        {
            get => _isPreGame;
            set
            {
                _isPreGame = value;
                OnPropertyChanged(nameof(IsPreGame));
            }
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                _isPlaying = value;
                OnPropertyChanged(nameof(IsPlaying));
            }
        }

        public bool ShowFeedback
        {
            get => _showFeedback;
            set
            {
                _showFeedback = value;
                OnPropertyChanged(nameof(ShowFeedback));
            }
        }

        public QuizQuestion CurrentQuestion
        {
            get => _currentQuestion;
            set
            {
                _currentQuestion = value;
                OnPropertyChanged(nameof(CurrentQuestion));
            }
        }

        public double Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                OnPropertyChanged(nameof(Progress));
            }
        }

        public string ProgressText
        {
            get => _progressText;
            set
            {
                _progressText = value;
                OnPropertyChanged(nameof(ProgressText));
            }
        }

        public string QuestionCounterText
        {
            get => _questionCounterText;
            set
            {
                _questionCounterText = value;
                OnPropertyChanged(nameof(QuestionCounterText));
            }
        }

        public string FeedbackText
        {
            get => _feedbackText;
            set
            {
                _feedbackText = value;
                OnPropertyChanged(nameof(FeedbackText));
            }
        }

        public string FeedbackIcon
        {
            get => _feedbackIcon;
            set
            {
                _feedbackIcon = value;
                OnPropertyChanged(nameof(FeedbackIcon));
            }
        }

        public string FeedbackColor
        {
            get => _feedbackColor;
            set
            {
                _feedbackColor = value;
                OnPropertyChanged(nameof(FeedbackColor));
            }
        }

        public ICommand StartQuizCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand AnswerCommand { get; }

        public QuizPageViewModel()
        {
            StartQuizCommand = new Command(async () => await StartQuiz());
            GoBackCommand = new Command(async () => await GoBack());
            AnswerCommand = new Command<string>(async (answer) => await ProcessAnswer(answer));
        }

        private async Task StartQuiz()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Starting quiz for category: {CategoryId}");

                // Try main method first
                _questions = await FirebaseServices.GetQuizQuestionsByCategoryAsync(CategoryId);

                // If no questions found, try simple method
                if (_questions == null || _questions.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Main method returned no questions, trying simple method...");
                    _questions = await FirebaseServices.GetQuizQuestionsByCategorySimpleAsync(CategoryId);
                }

                // If still no questions, try nested array method
                if (_questions == null || _questions.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Simple method returned no questions, trying nested array method...");
                    _questions = await FirebaseServices.GetQuestionsFromNestedArrayAsync(CategoryId);
                }

                if (_questions == null || _questions.Count == 0)
                {
                    // Debug the structure
                    var debugInfo = await FirebaseServices.DebugQuestionStructureAsync();
                    System.Diagnostics.Debug.WriteLine(debugInfo);

                    System.Diagnostics.Debug.WriteLine($"No questions found for category {CategoryId}");
                    await Application.Current.MainPage.DisplayAlert(
                        "Error",
                        $"No questions found for category {CategoryId}.\n\nPlease check:\n1. Questions exist in Firebase\n2. Structure is Question/{CategoryId}/[questions]\n\nDebug info printed to console.",
                        "OK");
                    await GoBack();
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"Successfully loaded {_questions.Count} questions");
                foreach (var q in _questions)
                {
                    System.Diagnostics.Debug.WriteLine($"  - {q.QuestionText}");
                }

                // Start the quiz
                _currentQuestionIndex = 0;
                _correctAnswers = 0;
                IsPreGame = false;
                IsPlaying = true;
                LoadNextQuestion();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error starting quiz: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    $"Failed to load questions: {ex.Message}",
                    "OK");
            }
        }

        private void LoadNextQuestion()
        {
            if (_currentQuestionIndex < _questions.Count)
            {
                CurrentQuestion = _questions[_currentQuestionIndex];
                UpdateProgress();
                QuestionCounterText = $"Question {_currentQuestionIndex + 1} of {_questions.Count}";
            }
            else
            {
                // Quiz completed
                EndQuiz();
            }
        }

        private void UpdateProgress()
        {
            Progress = (double)_currentQuestionIndex / _questions.Count;
            ProgressText = $"{_currentQuestionIndex}/{_questions.Count} completed";
        }

        private async Task ProcessAnswer(string selectedAnswer)
        {
            if (string.IsNullOrEmpty(selectedAnswer) || CurrentQuestion == null)
                return;

            // Check if answer is correct
            bool isCorrect = selectedAnswer == CurrentQuestion.CorrectAnswer;

            if (isCorrect)
            {
                _correctAnswers++;
                FeedbackText = "Correct!";
                FeedbackIcon = "✓";
                FeedbackColor = "#4CAF50";
            }
            else
            {
                FeedbackText = "Wrong!";
                FeedbackIcon = "✗";
                FeedbackColor = "#F44336";
            }

            // Show feedback
            ShowFeedback = true;

            // Wait for 1 second
            await Task.Delay(1000);

            // Hide feedback
            ShowFeedback = false;

            // Move to next question
            _currentQuestionIndex++;
            LoadNextQuestion();
        }

        private async void EndQuiz()
        {
            IsPlaying = false;

            // Calculate results
            double percentage = (_correctAnswers * 100.0) / _questions.Count;

            // Navigate to results page
            await Shell.Current.GoToAsync($"ResultsPage?correct={_correctAnswers}&total={_questions.Count}&percentage={percentage:F1}");
        }

        private async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}