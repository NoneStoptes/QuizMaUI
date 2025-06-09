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
        private bool _isSpeaking = false;
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
        private CancellationTokenSource _cancellationTokenSource;

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

        public bool IsSpeaking
        {
            get => _isSpeaking;
            set
            {
                _isSpeaking = value;
                OnPropertyChanged(nameof(IsSpeaking));
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
        public ICommand SpeakQuestionCommand { get; }
        public ICommand SpeakAnswersCommand { get; }
        public ICommand SpeakAllCommand { get; }
        public ICommand StopSpeakingCommand { get; }

        public QuizPageViewModel()
        {
            StartQuizCommand = new Command(async () => await StartQuiz());
            GoBackCommand = new Command(async () => await GoBack());
            AnswerCommand = new Command<string>(async (answer) => await ProcessAnswer(answer));
            SpeakQuestionCommand = new Command(async () => await SpeakQuestion(), () => !IsSpeaking);
            SpeakAnswersCommand = new Command(async () => await SpeakAnswers(), () => !IsSpeaking);
            SpeakAllCommand = new Command(async () => await SpeakQuestionAndAnswers(), () => !IsSpeaking);
            StopSpeakingCommand = new Command(StopSpeaking, () => IsSpeaking);
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

            // Stop any ongoing speech when answering
            StopSpeaking();

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
            // Stop any ongoing speech
            StopSpeaking();

            IsPlaying = false;

            // Calculate results
            double percentage = (_correctAnswers * 100.0) / _questions.Count;

            // Navigate to results page
            await Shell.Current.GoToAsync($"ResultsPage?correct={_correctAnswers}&total={_questions.Count}&percentage={percentage:F1}");
        }

        private async Task GoBack()
        {
            // Stop any ongoing speech
            StopSpeaking();
            await Shell.Current.GoToAsync("..");
        }

        // TTS Methods
        private async Task SpeakQuestion()
        {
            if (CurrentQuestion == null || IsSpeaking)
                return;

            await SpeakText($"Question: {CurrentQuestion.QuestionText}");
        }

        private async Task SpeakAnswers()
        {
            if (CurrentQuestion == null || IsSpeaking)
                return;

            try
            {
                IsSpeaking = true;
                RefreshCommands();

                _cancellationTokenSource = new CancellationTokenSource();

                // Speak all answer options
                var options = new[] { CurrentQuestion.Option1, CurrentQuestion.Option2, CurrentQuestion.Option3, CurrentQuestion.Option4 };

                for (int i = 0; i < options.Length; i++)
                {
                    if (!string.IsNullOrEmpty(options[i]))
                    {
                        await SpeakTextInternal($"Option {i + 1}: {options[i]}", _cancellationTokenSource.Token);
                        await Task.Delay(300, _cancellationTokenSource.Token);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Speech was cancelled, this is normal
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TTS Error: {ex.Message}");
            }
            finally
            {
                IsSpeaking = false;
                RefreshCommands();
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        private async Task SpeakQuestionAndAnswers()
        {
            if (CurrentQuestion == null || IsSpeaking)
                return;

            try
            {
                IsSpeaking = true;
                RefreshCommands();

                _cancellationTokenSource = new CancellationTokenSource();

                // Speak question
                await SpeakTextInternal($"Question: {CurrentQuestion.QuestionText}", _cancellationTokenSource.Token);

                // Short pause
                await Task.Delay(500, _cancellationTokenSource.Token);

                // Speak all answer options
                var options = new[] { CurrentQuestion.Option1, CurrentQuestion.Option2, CurrentQuestion.Option3, CurrentQuestion.Option4 };

                for (int i = 0; i < options.Length; i++)
                {
                    if (!string.IsNullOrEmpty(options[i]))
                    {
                        await SpeakTextInternal($"Option {i + 1}: {options[i]}", _cancellationTokenSource.Token);
                        await Task.Delay(300, _cancellationTokenSource.Token);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Speech was cancelled, this is normal
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TTS Error: {ex.Message}");
            }
            finally
            {
                IsSpeaking = false;
                RefreshCommands();
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        private async Task SpeakText(string text)
        {
            if (IsSpeaking)
                return;

            try
            {
                IsSpeaking = true;
                RefreshCommands();

                _cancellationTokenSource = new CancellationTokenSource();
                await SpeakTextInternal(text, _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                // Speech was cancelled, this is normal
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TTS Error: {ex.Message}");
            }
            finally
            {
                IsSpeaking = false;
                RefreshCommands();
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        private async Task SpeakTextInternal(string text, CancellationToken cancellationToken)
        {
            var speechOptions = new SpeechOptions()
            {
                Volume = 0.75f,
                Pitch = 1.0f
            };

            await TextToSpeech.Default.SpeakAsync(text, speechOptions, cancellationToken);
        }

        private void StopSpeaking()
        {
            if (_cancellationTokenSource != null && !_cancellationTokenSource.Token.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
            }

            IsSpeaking = false;
            RefreshCommands();
        }

        private void RefreshCommands()
        {
            ((Command)SpeakQuestionCommand).ChangeCanExecute();
            ((Command)SpeakAnswersCommand).ChangeCanExecute();
            ((Command)SpeakAllCommand).ChangeCanExecute();
            ((Command)StopSpeakingCommand).ChangeCanExecute();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}